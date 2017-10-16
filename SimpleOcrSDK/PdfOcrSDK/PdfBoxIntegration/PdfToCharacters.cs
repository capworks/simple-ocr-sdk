using System.Collections.Generic;
using org.apache.pdfbox.cos;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;
using PdfOcrSDK.Model;

namespace PdfOcrSDK.PdfBoxIntegration
{
    internal class PdfToCharacters : PDFTextStripper
    {
        private readonly List<CharItem> _items = new List<CharItem>();

        public PdfToCharacters()
        {
            base.setSortByPosition(true);
        }

        public List<CharItem> GetItems(PDPage aPage, PDResources resources, COSStream cosStream)
        {
            processStream(aPage, resources, cosStream);
            return _items;
        }

        protected override void processTextPosition(TextPosition text)
        {
            _items.Add(new CharItem(text.getCharacter(), text.getY(), text.getHeight(),
                text.getX(), text.getWidth(), text.getWidthOfSpace()));
        }
    }
}
