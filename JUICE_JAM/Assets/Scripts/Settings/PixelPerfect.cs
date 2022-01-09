﻿namespace JuiceJam.Settings
{
    using System.Xml.Linq;

    public class PixelPerfect : BoolSetting
    {
        public const string SAVE_ELEMENT_NAME = "PixelPerfect";

        public PixelPerfect() : base()
        {
        }

        public PixelPerfect(XElement element) : base(element)
        {
        }

        public override string SaveElementName => SAVE_ELEMENT_NAME;

        public override void Init()
        {
            Value = true;
        }
    }
}