using System;
using java.io;
using java.util;
using org.apache.pdfbox.exceptions;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.pdmodel.common;
using PdfOcrSDK.Exceptions;
using PdfOcrSDK.Model;

namespace PdfOcrSDK.PdfBoxIntegration
{
    internal class ExtractPdf
    {
        public PdfOcrResult Execute(byte[] bytes)
        {
            PDDocument document = null;
            try
            {
                LoadPdf(bytes, ref document);

                List allPages = document.getDocumentCatalog().getAllPages();
                if (allPages.size() == 0) throw new PdfNotReadableException("Pdf contains no readable content");

                //only first page
                PDPage page = (PDPage) allPages.get(0);

                PDStream contents = page.getContents();
                if (contents == null) throw new PdfNotReadableException("Pdf contains no readable content");

                var items = new PdfToCharacters().GetItems(page, page.findResources(), page.getContents().getStream());
                if (items.Count == 0) throw new PdfNotReadableException("Pdf contains no readable content");

                var mediaBox = page.findMediaBox();
               
                var height = mediaBox?.getHeight() ?? 0;
                var width = mediaBox?.getWidth() ?? 0;
                var itemsArray = items.ToArray();

                var keywords = "";
                try
                {
                    keywords = document.getDocumentInformation()?.getKeywords();
                }
                catch(Exception) { } // we do not know if PDF box can fail on this, if there is no keywords or something else. We dont really care we just want the keywords if possible. 

                return new PdfOcrResult() { Items = itemsArray, Height = height, Width = width, Keywords = keywords};
            }
            catch (PdfReadException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new PdfReadException("Pdf could not be loaded. It is not a redable pdf.", e);
            }
            finally
            {
                document?.close();
            }
        }

        private void LoadPdf(byte[] bytes, ref PDDocument document)
        {
            try
            {
                InputStream ins = new ByteArrayInputStream(bytes);
                document = PDDocument.loadNonSeq(ins, null);
            }
            catch (Exception)
            {
                try
                {
                    InputStream ins = new ByteArrayInputStream(bytes);
                    document = PDDocument.load(ins, null, true);
                }
                catch (IOException e)
                {
                    throw new PdfNotReadableException($"The pdf could not be loaded correctly. The fileformat might be corrupted.", e);
                }
                catch (Exception e)
                {
                    throw new PdfReadException($"Could not load byte array of pdf", e);
                }
            }

            if (document.isEncrypted())
            {
                try
                {
                    document.decrypt("");
                }
                catch (InvalidPasswordException e)
                {
                    throw new PdfNotReadableException($"The pdf is password protected.'", e);
                }
            }
        }
    }
}
