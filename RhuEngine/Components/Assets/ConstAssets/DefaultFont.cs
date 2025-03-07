﻿using RhuEngine.Linker;
using RhuEngine.WorldObjects.ECS;

namespace RhuEngine.Components
{
	[Category(new string[] { "Assets/ConstAssets" })]
	public class DefaultFont : AssetProvider<RFont>
	{
		RFont _font;
		private void LoadFont() {
			if (!Engine.EngineLink.CanRender) {
				return;
			}
			_font = RFont.Default;
			Load(_font);
		}
		public override void OnLoaded() {
			base.OnLoaded();
			LoadFont();
		}
	}
}
