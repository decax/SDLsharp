using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows;

namespace SDL
{
	public class Renderer : IDisposable
	{
		/// <summary>
		/// Information on the capabilities of a render driver or context.
		/// </summary>
		public class Info
		{
			public string Name { get; }
			public Type Flags { get; }
			public List<PixelFormat> TextureFormats { get; } = new List<PixelFormat>();
			public Size MaxTextureSize { get; }

			public Info(SDL_RendererInfo rendererInfo)
			{
				Name = Marshal.PtrToStringAnsi(rendererInfo.name);
				Flags = (Type)rendererInfo.flags;

				for (int i = 0; i < rendererInfo.num_texture_formats; i++)
				{
					unsafe
					{
						TextureFormats.Add((PixelFormat)rendererInfo.texture_formats[i]);
					}
				}

				MaxTextureSize = new Size(rendererInfo.max_texture_width, rendererInfo.max_texture_height);
			}
		}

		/// <summary>
		/// Flags used when creating a rendering context
		/// </summary>
		[Flags]
		public enum Type
		{
			Software      = 0x00000001, 
			Accelerated   = 0x00000002, 
			PresentVSync  = 0x00000004, 
			TargetTexture = 0x00000008
		}

		/// <summary>
		/// The blend mode used in Copy() and drawing operations.
		/// </summary>
		public enum BlendModeType
		{
			None  = 0x00000001, // no blending
                                //   dstRGBA = srcRGBA
			Blend = 0x00000001, // alpha blending
                                //   dstRGB = (srcRGB * srcA) + (dstRGB * (1-srcA))
                                //   dstA = srcA + (dstA * (1-srcA))
			Add   = 0x00000002, // additive blending
                                //   dstRGB = (srcRGB * srcA) + dstRGB
                                //   dstA = dstA */
			Mod   = 0x00000004	// color modulate
			                    //   dstRGB = srcRGB * dstRGB
		}                                     
      

		public Renderer(Window window, Type type)
		{
			sdlRenderer = SDL_CreateRenderer(window.sdlWindow, -1, (uint)type);
		}

		public void Dispose()
		{
			SDL_DestroyRenderer(sdlRenderer);
		}

		/// <summary>
		/// Clear the current rendering target with the drawing color
		/// </summary>
		public void Clear()
		{
			SDL_RenderClear(sdlRenderer);
		}

		/// <summary>
		/// Get information about all 2D rendering drivers for the current display.
		/// </summary>
		/// <returns>The render driver info.</returns>
		public static List<Info> GetRenderDriverInfo()
		{
			var renderDriverInfos = new List<Info>();

			int numRenderDrivers = SDL_GetNumRenderDrivers();

			for (int i = 0; i < numRenderDrivers; i++) {
				var renderDriverInfo = new SDL_RendererInfo();
				SDL_GetRenderDriverInfo(i, out renderDriverInfo);
				renderDriverInfos.Add(new Info(renderDriverInfo));
			}

			return renderDriverInfos;
		}

		/// <summary>
		/// Get information about a rendering context.
		/// </summary>
		/// <returns>The info.</returns>
		public Info GetInfo()
		{
			var rendererInfo = new SDL_RendererInfo();
			SDL_GetRendererInfo(sdlRenderer, out rendererInfo);

			return new Info(rendererInfo);
		}

		/// <summary>
		/// Gets or sets the color used for drawing operations (Rect, Line and Clear).
		/// </summary>
		/// <value>The color used to draw on the rendering target.</value>
		public Color DrawColor
		{
			get
			{
				byte r, g, b, a;
				SDL_GetRenderDrawColor(sdlRenderer, out r, out g, out b, out a);
				return Color.FromArgb(a, r, g, b);
			}
			set
			{
				SDL_SetRenderDrawColor(sdlRenderer, value.R, value.G, value.B, value.A);
			}
		}

		/// <summary>
		/// Gets or sets the blend mode used for drawing operations.
		/// </summary>
		/// <value>The blend mode.</value>
		public BlendModeType BlendMode
		{
			get
			{
				int blendMode;
				SDL_GetRenderDrawBlendMode(sdlRenderer, out blendMode);
				return (BlendModeType)blendMode;
			}
			set
			{
				SDL_SetRenderDrawBlendMode(sdlRenderer, (int)value);
			}
		}

		/// <summary>
		/// Determines whether the renderer supports the use of render targets
		/// </summary>
		/// <value><c>true</c> if supported, <c>false</c> if not.</value>
		public bool IsRenderTargetSupported
		{
			get
			{
				return SDL_RenderTargetSupported(sdlRenderer) == 1;
			}
		}

		public void SetRenderTarget(Texture texture)
		{
			SDL_SetRenderTarget(sdlRenderer, texture.sdlTexture);
		}

		public Texture GetRenderTarget()
		{
			var sdlTexture = SDL_GetRenderTarget(sdlRenderer);
			return new Texture(this, sdlTexture);
		}

		/// <summary>
		/// Gets or sets device independent resolution for rendering
		/// </summary>
		/// <value>The size of the logical resolution.</value>
		public Size LogicalSize
		{
			get
			{
				int width, height;
				SDL_RenderGetLogicalSize(sdlRenderer, out width, out height);
				return new Size(width, height);
			}
			set
			{
				SDL_RenderSetLogicalSize(sdlRenderer, value.Width, value.Height);
			}
		}

		/// <summary>
		/// Draw a point on the current rendering target.
		/// </summary>
		/// <param name="point">The point.</param>
		public void DrawPoint(Point point)
		{
			SDL_RenderDrawPoint(sdlRenderer, point.X, point.Y);
		}

		/// <summary>
		/// Draw a line on the current rendering target.
		/// </summary>
		/// <param name="startPoint">The start point.</param>
		/// <param name="endPoint">The end point.</param>
		public void DrawLine(Point startPoint, Point endPoint)
		{
			SDL_RenderDrawLine(sdlRenderer, startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
		}

		/// <summary>
		/// Draw a rectangle on the current rendering target.
		/// </summary>
		/// <param name="rect">The destination rectangle.</param>
		public void DrawRect(Rectangle rect)
		{
			var sdlRect = new SDL_Rect { x = rect.X, y = rect.Y, w = rect.Width, h = rect.Height };

			SDL_RenderDrawRect(sdlRenderer, ref sdlRect);
		}

		/// <summary>
		/// Fill a rectangle on the current rendering target with the drawing color.
		/// </summary>
		/// <param name="rect">The destination rectangle.</param>
		public void FillRect(Rectangle rect)
		{
			var sdlRect = new SDL_Rect { x = rect.X, y = rect.Y, w = rect.Width, h = rect.Height };

			SDL_RenderFillRect(sdlRenderer, ref sdlRect);
		}

		/// <summary>
		/// Copy a portion of the texture to the current rendering target.
		/// </summary>
		/// <param name="texture">The source texture.</param>
		/// <param name="position">The destination point.</param>
		public void Copy(Texture texture, Point position)
		{
			var srcRect = new SDL_Rect { x = 0, y = 0, w = texture.Size.Width, h = texture.Size.Height };
			var dstRect = new SDL_Rect { x = position.X, y = position.Y, w = texture.Size.Width, h = texture.Size.Height };

			SDL_RenderCopy(sdlRenderer, texture.sdlTexture, ref srcRect, ref dstRect);
		}

		public Color[] ReadPixels()
		{
			// TODO
			//SDL_Rect rect;
			//SDL_RenderReadPixels(sdlRenderer, ref rect, uint format, IntPtr pixels, int pitch);

			return new Color[1];
		}

		/// <summary>
		/// Update the screen with rendering performed.
		/// </summary>
		public void Present()
		{
			SDL_RenderPresent(sdlRenderer);
		}

		public IntPtr sdlRenderer;

		#region Native
		struct SDL_Rect
		{
			public int x, y, w, h;
		}

		public unsafe struct SDL_RendererInfo
		{
			public IntPtr name;
			public uint flags;
			public uint num_texture_formats;
			public fixed uint texture_formats[16];
			public int max_texture_width;
			public int max_texture_height;
		}

		const string sdlDLL = "/Library/Frameworks/SDL2.framework/SDL2";

		[DllImport(sdlDLL)]
		private static extern IntPtr SDL_CreateRenderer(IntPtr window, int index, uint flags);

		[DllImport(sdlDLL)]
		private static extern void SDL_DestroyRenderer(IntPtr renderer);
		
		[DllImport(sdlDLL)]
		private static extern int SDL_GetNumRenderDrivers();

		[DllImport(sdlDLL)]
		private static extern int SDL_GetRenderDriverInfo(int index, out SDL_RendererInfo info);

		[DllImport(sdlDLL)]
		private static extern int SDL_GetRendererInfo(IntPtr renderer, out SDL_RendererInfo info);

		[DllImport(sdlDLL)]
		private static extern int SDL_RenderClear(IntPtr renderer);

		[DllImport(sdlDLL)]
		private static extern int SDL_SetRenderDrawColor(IntPtr renderer, byte r, byte g, byte b, byte a);

		[DllImport(sdlDLL)]
		private static extern int SDL_GetRenderDrawColor(IntPtr renderer, out byte r, out byte g, out byte b, out byte a);

		[DllImport(sdlDLL)]
		private static extern int SDL_SetRenderDrawBlendMode(IntPtr renderer, int blendMode);

		[DllImport(sdlDLL)]
		private static extern int SDL_GetRenderDrawBlendMode(IntPtr renderer, out int blendMode);

		[DllImport(sdlDLL)]
		private static extern void SDL_RenderPresent(IntPtr renderer);

		[DllImport(sdlDLL)]
		private static extern int SDL_RenderTargetSupported(IntPtr renderer);

		[DllImport(sdlDLL)]
		private static extern int SDL_SetRenderTarget(IntPtr renderer, IntPtr texture);

		[DllImport(sdlDLL)]
		private static extern IntPtr SDL_GetRenderTarget(IntPtr renderer);

		[DllImport(sdlDLL)]
		private static extern int SDL_RenderSetLogicalSize(IntPtr renderer, int w, int h);

		[DllImport(sdlDLL)]
		private static extern void SDL_RenderGetLogicalSize(IntPtr renderer, out int w, out int h);

		[DllImport(sdlDLL)]
		private static extern int SDL_RenderCopy(IntPtr renderer, IntPtr texture, ref SDL_Rect srcrect, ref SDL_Rect dstrect);

		[DllImport(sdlDLL)]
		private static extern int SDL_RenderDrawPoint(IntPtr renderer, int x, int y);

		[DllImport(sdlDLL)]
		private static extern int SDL_RenderDrawLine(IntPtr renderer, int x1, int y1, int x2, int y2);

		[DllImport(sdlDLL)]
		private static extern int SDL_RenderDrawRect(IntPtr renderer, ref SDL_Rect rect);

		[DllImport(sdlDLL)]
		private static extern int SDL_RenderFillRect(IntPtr renderer, ref SDL_Rect rect);

		[DllImport(sdlDLL)]
		private static extern int SDL_RenderReadPixels(IntPtr renderer, ref SDL_Rect rect, uint format, IntPtr pixels, int pitch);
		#endregion
	}

}

