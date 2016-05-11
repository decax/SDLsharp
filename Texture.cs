using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace SDL
{
	public class Texture : IDisposable
	{
		public enum Access
		{
			Static,    // Changes rarely, not lockable
			Streaming, // Changes frequently, lockable
			Target     // Texture can be used as a render target
		}

		public Texture(Renderer renderer)
		{
			this.renderer = renderer;
		}

		public Texture(Renderer renderer, IntPtr sdlTexture)
		{
			this.renderer = renderer;
			this.sdlTexture = sdlTexture;

			uint format;
			int access;
			int width, height;
			SDL_QueryTexture(sdlTexture, out format, out access, out width, out height);

			Size = new Size(width, height);
		}

		public Texture(Renderer renderer, Surface surface)
		{
			sdlTexture = SDL_CreateTextureFromSurface(renderer.sdlRenderer, surface.sdlSurface);

			uint format;
			int access;
			int width, height;
			SDL_QueryTexture(sdlTexture, out format, out access, out width, out height);

			Size = new Size(width, height);
		}

		public Texture(Renderer renderer, Size size)
			: this(renderer, PixelFormat.ABGR_8888, Access.Static, size)
		{
		}

		public Texture(Renderer renderer, PixelFormat format, Access access, Size size)
		{
			this.renderer = renderer;
			this.Size = size;

			sdlTexture = SDL_CreateTexture(renderer.sdlRenderer, (uint)format, (int)access, size.Width, size.Height);
		}

		public void Dispose()
		{
			SDL_DestroyTexture(sdlTexture);
		}

		public void SetColorMod(Color color)
		{
			SDL_SetTextureColorMod(sdlTexture, color.R, color.G, color.B);
		}

		public Size Size { get; private set; }

		Renderer renderer;

		internal IntPtr sdlTexture;

		#region Native
		const string sdlDLL = "/Library/Frameworks/SDL2.framework/SDL2";

		[DllImport(sdlDLL)]
		private static extern IntPtr SDL_CreateTexture(IntPtr renderer, uint format, int access, int w, int h);

		[DllImport(sdlDLL)]
		private static extern IntPtr SDL_CreateTextureFromSurface(IntPtr renderer, IntPtr surface);

		[DllImport(sdlDLL)]
		private static extern void SDL_DestroyTexture(IntPtr texture);

		[DllImport(sdlDLL)]
		private static extern int SDL_SetTextureColorMod(IntPtr texture, byte r, byte g, byte b);

		[DllImport(sdlDLL)]
		private static extern int SDL_QueryTexture(IntPtr texture, out uint format, out int access,	out int w, out int h);
		#endregion
	}
}

