using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SDL
{
	public class Surface : IDisposable
	{
		[Flags]
		public enum Flag
		{
			SwSurface = 0, 
			PreAlloc = 0x00000001, 
			RleAccel = 0x00000002, 
			DontFree = 0x00000004
		}

		public Surface()
		{
		}

		public Surface(IntPtr sdlSurface)
		{
			this.sdlSurface = sdlSurface;

			var sdlSurfaceStruct = Marshal.PtrToStructure<SDL_Surface>(sdlSurface);

			Size = new Size(sdlSurfaceStruct.w, sdlSurfaceStruct.h);
		}

		public void Dispose()
		{
			Free();
		}

		public void Create(Flag flags, Size size, int depth, uint rMask, uint gMask, uint bMask, uint aMask)
		{
			sdlSurface = SDL_CreateRGBSurface((uint)flags, size.Width, size.Height, depth, rMask, gMask, bMask, aMask);

			Size = size;
		}

		public void Free()
		{
			SDL_FreeSurface(sdlSurface);
		}

		public void LoadBMP(string filename)
		{
			//sdlSurface = SDL_LoadBMP_RW(SDL_RWFromFile(filename, "rb"), 1);
		}

		public Size Size { get; private set; }

		public static void Blit(Surface source, Rectangle sourceRect, Surface destination, Rectangle destinationRect)
		{
			var sdlSourceRect      = new SDL_Rect() { x = sourceRect.X, y = sourceRect.Y, w = sourceRect.Width, h = sourceRect.Height };
			var sdlDestinationRect = new SDL_Rect() { x = destinationRect.X, y = destinationRect.Y, w = destinationRect.Width, h = destinationRect.Height };

			SDL_UpperBlit(source.sdlSurface, ref sdlSourceRect, destination.sdlSurface, ref sdlDestinationRect);
		}

		public IntPtr sdlSurface;

		#region Native
		struct SDL_Rect
		{
			public int x, y, w, h;
		}

		struct SDL_Surface
		{
			uint flags;               /**< Read-only */
			IntPtr format;    /**< Read-only */
			public int w, h;                   /**< Read-only */
			int pitch;                  /**< Read-only */
			IntPtr pixels;               /**< Read-write */

			/** Application data associated with the surface */
			IntPtr userdata;             /**< Read-write */

			/** information needed for surfaces requiring locks */
			int locked;                 /**< Read-only */
			IntPtr lock_data;            /**< Read-only */

			/** clipping information */
			SDL_Rect clip_rect;         /**< Read-only */

			/** info for fast blit mapping to other surfaces */
			IntPtr map;    /**< Private */

			/** Reference count -- used when freeing surface */
			int refcount;               /**< Read-mostly */
		}

		const string sdlDLL = "/Library/Frameworks/SDL2.framework/SDL2";

		[DllImport(sdlDLL)]
		private static extern IntPtr SDL_CreateRGBSurface(uint flags, int width, int height, int depth, uint Rmask, uint Gmask, uint Bmask, uint Amask);

		[DllImport(sdlDLL)]
		private static extern IntPtr SDL_CreateRGBSurfaceFrom(IntPtr pixels, int width, int height, int depth, int pitch, uint Rmask, uint Gmask, uint Bmask, uint Amask);

		[DllImport(sdlDLL)]
		private static extern void SDL_FreeSurface(IntPtr surface);

		[DllImport(sdlDLL)]
		private static extern IntPtr SDL_LoadBMP_RW(IntPtr src, int freesrc);

		[DllImport(sdlDLL)]
		private static extern int SDL_UpperBlit(IntPtr src, ref SDL_Rect srcrect, IntPtr dst, ref SDL_Rect dstrect);

		[DllImport(sdlDLL)]
		private static extern IntPtr SDL_RWFromFile(string file, string mode);
		#endregion
	}
}

