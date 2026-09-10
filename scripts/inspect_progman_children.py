import ctypes
from ctypes import wintypes
u=ctypes.WinDLL('user32', use_last_error=True)
prog=u.FindWindowW('Progman', None)
print(f'PROGMAN=0x{prog:X}')
getlong=u.GetWindowLongPtrW if ctypes.sizeof(ctypes.c_void_p)==8 else u.GetWindowLongW
getlong.restype=ctypes.c_longlong if ctypes.sizeof(ctypes.c_void_p)==8 else ctypes.c_long
getlong.argtypes=[wintypes.HWND, ctypes.c_int]

def cls(h):
    b=ctypes.create_unicode_buffer(128); u.GetClassNameW(h,b,128); return b.value

def rect(h):
    r=wintypes.RECT(); u.GetWindowRect(h,ctypes.byref(r)); return (r.left,r.top,r.right,r.bottom)

def pid(h):
    p=wintypes.DWORD(); u.GetWindowThreadProcessId(h,ctypes.byref(p)); return p.value

print(f'PROGMAN_EX=0x{getlong(prog,-20)&0xffffffffffffffff:X}')
child=0; i=0
while True:
    child=u.FindWindowExW(prog, child, None, None)
    if not child: break
    style=getlong(child,-16)&0xffffffffffffffff
    ex=getlong(child,-20)&0xffffffffffffffff
    print(f'{i}: hwnd=0x{child:X} cls={cls(child)!r} pid={pid(child)} vis={bool(u.IsWindowVisible(child))} rect={rect(child)} style=0x{style:X} ex=0x{ex:X}')
    i+=1
