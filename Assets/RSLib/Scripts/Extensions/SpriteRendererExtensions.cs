namespace RSLib.Extensions
{
	using UnityEngine;

	public static class SpriteRendererExtensions
	{
		#region FLIP

		/// <summary>
		/// Flips the SpriteRenderer on the X axis.
		/// </summary>
		public static void FlipX(this SpriteRenderer spriteRenderer)
		{
			spriteRenderer.flipX = !spriteRenderer.flipX;
		}

		/// <summary>
		/// Flips the SpriteRenderer on the Y axis.
		/// </summary>
		public static void FlipY(this SpriteRenderer spriteRenderer)
		{
			spriteRenderer.flipY = !spriteRenderer.flipY;
		}

		#endregion FLIP

		#region SET COLOR

		/// <summary>
		/// Sets the SpriteRenderer color.
		/// </summary>
		public static void SetColor(this SpriteRenderer spriteRenderer, Color color)
		{
			spriteRenderer.color = color;
		}

		/// <summary>
		/// Sets the SpriteRenderer color's red channel value.
		/// </summary>
		public static void SetColorRedValue(this SpriteRenderer spriteRenderer, float r)
		{
			spriteRenderer.color = new Color(r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a);
		}

		/// <summary>
		/// Sets the SpriteRenderer color's green channel value.
		/// </summary>
		public static void SetColorGreenValue(this SpriteRenderer spriteRenderer, float g)
		{
			spriteRenderer.color = new Color(spriteRenderer.color.r, g, spriteRenderer.color.b, spriteRenderer.color.a);
		}

		/// <summary>
		/// Sets the SpriteRenderer color's blue channel value.
		/// </summary>
		public static void SetColorBlueValue(this SpriteRenderer spriteRenderer, float b)
		{
			spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, b, spriteRenderer.color.a);
		}

		/// <summary>
		/// Sets the SpriteRenderer color's alpha.
		/// </summary>
		public static void SetAlpha(this SpriteRenderer spriteRenderer, float a)
		{
			spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, a);
		}

		#endregion SET COLOR
	}
}