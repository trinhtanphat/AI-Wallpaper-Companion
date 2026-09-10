using AIWallpaper.Desktop;

namespace AIWallpaper;

public sealed class InteractionForm : Form
{
    private readonly RichTextBox _messages;
    private readonly TextBox _input;
    private readonly Button _sendButton;
    private readonly Button _closeButton;

    public event EventHandler? Dismissed;

    public InteractionForm()
    {
        Text = "LUNA Desktop AI";
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        TopMost = true;
        ClientSize = new Size(430, 438);
        BackColor = Color.FromArgb(10, 18, 38);
        ForeColor = Color.White;
        Opacity = 0.97;
        KeyPreview = true;
        Padding = new Padding(18);

        var title = new Label
        {
            Text = "L U N A",
            Font = new Font("Segoe UI Semibold", 14f),
            ForeColor = Color.FromArgb(238, 246, 255),
            AutoSize = true,
            Location = new Point(20, 16)
        };

        var subtitle = new Label
        {
            Text = "LOCAL DESKTOP AI",
            Font = new Font("Segoe UI", 8f),
            ForeColor = Color.FromArgb(126, 213, 255),
            AutoSize = true,
            Location = new Point(112, 22)
        };

        _closeButton = new Button
        {
            Text = "×",
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 13f),
            ForeColor = Color.FromArgb(190, 210, 235),
            BackColor = Color.FromArgb(10, 18, 38),
            Size = new Size(34, 30),
            Location = new Point(378, 10),
            TabStop = false
        };
        _closeButton.FlatAppearance.BorderSize = 0;
        _closeButton.Click += (_, _) => Dismiss();

        _messages = new RichTextBox
        {
            ReadOnly = true,
            BorderStyle = BorderStyle.None,
            BackColor = Color.FromArgb(17, 27, 54),
            ForeColor = Color.FromArgb(229, 239, 255),
            Font = new Font("Segoe UI", 10f),
            Location = new Point(20, 58),
            Size = new Size(390, 285),
            DetectUrls = false
        };

        _input = new TextBox
        {
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(8, 14, 30),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10.5f),
            Location = new Point(20, 362),
            Size = new Size(300, 32),
            PlaceholderText = "Nhắn cho Luna..."
        };

        _sendButton = new Button
        {
            Text = "Gửi",
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI Semibold", 10f),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(111, 92, 245),
            Size = new Size(82, 34),
            Location = new Point(328, 360)
        };
        _sendButton.FlatAppearance.BorderSize = 0;
        _sendButton.Click += (_, _) => SendCurrentMessage();

        var hint = new Label
        {
            Text = "Ctrl+Alt+W để đóng · Esc để ẩn",
            Font = new Font("Segoe UI", 8f),
            ForeColor = Color.FromArgb(135, 165, 195),
            AutoSize = true,
            Location = new Point(20, 405)
        };

        Controls.AddRange([title, subtitle, _closeButton, _messages, _input, _sendButton, hint]);
        AcceptButton = _sendButton;
        KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                Dismiss();
            }
        };

        AppendMessage("Luna", "Mình đang sống trong hình nền desktop của bạn.");
        Shown += (_, _) => PositionAndFocus();
    }

    public InteractionFocusResult FocusInput()
    {
        PositionNearTaskbar();
        BringToFront();
        var foreground = InteractionWindowActivator.TryBringToForeground(Handle);
        Activate();
        _input.Select();
        var inputFocused = _input.Focus() && _input.Focused;
        foreground = foreground || InteractionWindowActivator.IsForeground(Handle);
        return new InteractionFocusResult(foreground, inputFocused);
    }

    private void PositionAndFocus()
    {
        _ = FocusInput();
    }

    private void PositionNearTaskbar()
    {
        var area = Screen.PrimaryScreen?.WorkingArea ?? SystemInformation.WorkingArea;
        var x = Math.Max(area.Left + 16, area.Right - Width - 28);
        var y = Math.Max(area.Top + 16, area.Bottom - Height - 28);
        Location = new Point(x, y);
    }

    private void SendCurrentMessage()
    {
        var text = _input.Text.Trim();
        if (text.Length == 0)
        {
            return;
        }

        AppendMessage("Bạn", text);
        _input.Clear();
        var reply = LocalChatResponder.GetReply(text);
        AppendMessage("Luna", reply);
        _input.Focus();
    }

    private void AppendMessage(string speaker, string text)
    {
        if (_messages.TextLength > 0)
        {
            _messages.AppendText(Environment.NewLine + Environment.NewLine);
        }
        _messages.SelectionStart = _messages.TextLength;
        _messages.SelectionColor = speaker == "Luna"
            ? Color.FromArgb(147, 205, 255)
            : Color.FromArgb(213, 181, 255);
        _messages.SelectionFont = new Font("Segoe UI Semibold", 9.5f);
        _messages.AppendText(speaker + Environment.NewLine);
        _messages.SelectionColor = Color.FromArgb(232, 239, 249);
        _messages.SelectionFont = new Font("Segoe UI", 10f);
        _messages.AppendText(text);
        _messages.SelectionStart = _messages.TextLength;
        _messages.ScrollToCaret();
    }

    public void Dismiss()
    {
        Hide();
        Dismissed?.Invoke(this, EventArgs.Empty);
    }
}
