using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SDL
{
	public class Window : IDisposable
	{
		public Window(string title, Size size)
		{
			sdlWindow = SDL_CreateWindow(title, 0, 0, size.Width, size.Height, 0);

			Size = size;
		}

		public void Dispose()
		{
			SDL_DestroyWindow(sdlWindow);
		}

		public Size Size;
		internal IntPtr sdlWindow;

		#region Native
		const string sdlDLL = "/Library/Frameworks/SDL2.framework/SDL2";

		[DllImport(sdlDLL)]
		private static extern IntPtr SDL_CreateWindow(string title, int x, int y, int w, int h, uint flags);

		[DllImport(sdlDLL)]
		private static extern void SDL_DestroyWindow(IntPtr window);
		#endregion
	}
}