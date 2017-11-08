using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TransymOcrSdk.Exceptions;

namespace TransymOcrSdk.Integration
{
    public class TransymAccess : TOCRdeclares
    {
        private TOCRJOBINFO2 JobInfo2;
        private int JobNo = TOCRCONFIG_DEFAULTJOB;
        public TransymAccess(string logFile)
        {
            JobInfo2 = new TOCRJOBINFO2
            {
                ProcessOptions =
                {
                    DisableCharacter = new short[256],
                    StructId = 1,
                    DeshadeOff = 1
                }
            };

            var fileInfo = new FileInfo(logFile);
            if (!fileInfo.Exists)
            {
                if (fileInfo.Directory != null && !fileInfo.Directory.Exists)
                {
                    Directory.CreateDirectory(fileInfo.Directory.FullName);
                }
                using (var str = File.Create(logFile))
                {
                    str.Close();
                }
            }

            //TOCRSetConfig(TOCRCONFIG_DEFAULTJOB, TOCRCONFIG_SRV_THREADPRIORITY, ThreadPriority.Highest));
            //var i1= TOCRSetConfig(TOCRCONFIG_DEFAULTJOB, TOCRCONFIG_DLL_MUTEXWAIT, 5000);
            //var i2 = TOCRSetConfig(TOCRCONFIG_DEFAULTJOB, TOCRCONFIG_DLL_EVENTWAIT, 5000);
            //var i3 = TOCRSetConfig(TOCRCONFIG_DEFAULTJOB, TOCRCONFIG_SRV_MUTEXWAIT, 5000);
            var i4 = TOCRSetConfig(TOCRCONFIG_DEFAULTJOB, TOCRCONFIG_DLL_ERRORMODE, TOCRERRORMODE_LOG);
            var i5 = TOCRSetConfigStr(TOCRCONFIG_DEFAULTJOB, TOCRCONFIG_LOGFILE, logFile);

            //InitTOCR();
        }

        private void RestartTOCR()
        {
            Stop();
            InitTOCR();
        }

        private void InitTOCR()
        {
            try
            {
                var Status = TOCRInitialise(ref JobNo);

                if (Status != TOCR_OK)
                    throw new TransymException("Unknown Transym Error. Could not initialize engine");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int Stop()
        {
            return TOCRShutdown(TOCRSHUTDOWNALL);
        }

        #region SDK Declares
        private const int DIB_PAL_COLORS = 1;
        private static uint DIB_RGB_COLORS = 0;
        private static uint BI_RGB = 0;
        private const int BI_BITFIELDS = 3;
        private const int PAGE_READWRITE = 4;
        private const int FILE_MAP_WRITE = 2;
        private const int CBM_INIT = 4;
        private const int SRCCOPY = 0x00CC0020;

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        struct RGBQUAD
        {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct BITMAPINFOHEADER
        {
            public uint biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public uint biCompression;
            public uint biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public uint biClrUsed;
            public uint biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct BITMAPINFO
        {
            public BITMAPINFOHEADER bmih;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public uint[] cols;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32", EntryPoint = "CreateFileMappingA", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CreateFileMappingMy(uint hFile, uint lpFileMappingAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, int lpName);

        [DllImport("kernel32", EntryPoint = "MapViewOfFile", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr MapViewOfFileMy(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

        [DllImport("kernel32", EntryPoint = "UnmapViewOfFile", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int UnmapViewOfFileMy(IntPtr lpBaseAddress);

        [DllImport("kernel32", EntryPoint = "RtlMoveMemory", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void CopyMemory(uint lpvDest, IntPtr lpvSrc, uint cbCopy);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GlobalFree(IntPtr hMem);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern int DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        private static extern int BitBlt(IntPtr hdcDst, int xDst, int yDst, int w, int h, IntPtr hdcSrc, int xSrc, int ySrc, int rop);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO bmi, uint Usage, out IntPtr bits, IntPtr hSection, uint dwOffset);

        #endregion

        public TOcrResultStructures.TOcrResults Ocr(string imgFile)
        {
            int error = 0;
            TOcrResultStructures.TOcrResults Results = new TOcrResultStructures.TOcrResults();

            JobInfo2.InputFile = imgFile;
            JobInfo2.JobType = TOCRJOBTYPE_TIFFFILE;

            if (!OCRWait(JobNo, JobInfo2, ref error))
                throw new TransymException("Unknown Transym Error. Engine did not respond. Transym error code: " + error);

            if (!GetResults(JobNo, ref Results, ref error))
                throw new TransymException("Transym Error. Could not get result. Transym error code: " + error);

            if (error != 0)
                throw new TransymException("Could not ocr stream. Transym error code: " + error);

            return Results;
        }

        // Wait for the engine to complete
        private bool OCRWait(int JobNo, TOCRJOBINFO2 JobInfo2, ref int error)
        {
            int Status = 0;
            int JobStatus = 0;
            int ErrorMode = 0;

            Status = TOCRDoJob2(JobNo, ref JobInfo2);


            if (Status != TOCR_OK) // 9=connection broken
            {
                RestartTOCR();
                Status = TOCRDoJob2(JobNo, ref JobInfo2);
            }

            if (Status == TOCR_OK)
            {
                Status = TOCRWaitForJob(JobNo, ref JobStatus);
            }
            else
            {
                throw new TransymException("Transym broken. Transym status: '" + Status + "'");
            }

            if (Status == TOCR_OK && JobStatus == TOCRJOBSTATUS_DONE)
            {
                return true;
            }
            else
            {
                // If something has gone wrong display a message
                // (Check that the OCR engine hasn't already displayed a message)
                TOCRGetConfig(JobNo, TOCRCONFIG_DLL_ERRORMODE, ref ErrorMode);
                if (ErrorMode == TOCRERRORMODE_NONE)
                {
                    TOCRGetJobStatus(JobNo, ref error);
                }
                return false;
            }
        }

        // OVERLOADED function to retrieve the results from the service process and load into 'Results'
        // Remember the character numbers returned refer to the Windows character set.
        private static bool GetResults(int JobNo, ref TOcrResultStructures.TOcrResults Results, ref int error)
        {
            int ResultsInf = 0; // number of bytes needed for results
            byte[] Bytes;
            int Offset;
            bool RetStatus = false;
            GCHandle BytesGC;
            IntPtr AddrOfItemBytes;

            Results.Hdr.NumItems = 0;
            if (TOCRGetJobResults(JobNo, ref ResultsInf, IntPtr.Zero) == TOCR_OK)
            {
                if (ResultsInf > 0)
                {
                    Bytes = new Byte[ResultsInf - 1];
                    // pin the Bytes array so that TOCRGetJobResults can write to it
                    BytesGC = GCHandle.Alloc(Bytes, GCHandleType.Pinned);
                    if (TOCRGetJobResults(JobNo, ref ResultsInf, BytesGC.AddrOfPinnedObject()) == TOCR_OK)
                    {
                        Results.Hdr = ((TOcrResultStructures.TOcrResultsHeader)(Marshal.PtrToStructure(BytesGC.AddrOfPinnedObject(), typeof(TOcrResultStructures.TOcrResultsHeader))));
                        if (Results.Hdr.NumItems > 0)
                        {
                            Results.Item = new TOcrResultStructures.TOcrResultsItem[Results.Hdr.NumItems];
                            Offset = Marshal.SizeOf(typeof(TOcrResultStructures.TOcrResultsHeader));
                            for (int ItemNo = 0; ItemNo <= Results.Hdr.NumItems - 1; ItemNo++)
                            {
                                AddrOfItemBytes = Marshal.UnsafeAddrOfPinnedArrayElement(Bytes, Offset);
                                Results.Item[ItemNo] = ((TOcrResultStructures.TOcrResultsItem)(Marshal.PtrToStructure(AddrOfItemBytes, typeof(TOcrResultStructures.TOcrResultsItem))));
                                Offset = Offset + Marshal.SizeOf(typeof(TOcrResultStructures.TOcrResultsItem));
                            }
                        }

                        RetStatus = true;

                        // Double check results
                        if (Results.Hdr.StructId == 0)
                        {
                            if (Results.Hdr.NumItems > 0)
                            {
                                if (Results.Item[0].StructId != 0)
                                {
                                    error = -1;//ResultStructureError;
                                    Results.Hdr.NumItems = 0;
                                    RetStatus = false;
                                }
                            }
                        }
                        else
                        {
                            error = -2;//ResultHeaderStructureError;
                            Results.Hdr.NumItems = 0;
                            RetStatus = false;
                        }
                    }
                    BytesGC.Free();
                }
            }
            return RetStatus;
        }

        // Demonstrates how to OCR an image using a memory mapped file created here
        public TOcrResultStructures.TOcrResults OcrByStream(Stream imgStream)
        {
            var status = TOCRInitialise(ref JobNo);
            if (status != TOCR_OK)
                throw new TransymException("Transym could not be started. Transym error code: " + status);

            try
            {
                int error = 0;
                Bitmap BMP;
                TOcrResultStructures.TOcrResults Results = new TOcrResultStructures.TOcrResults();
                JobInfo2.JobType = TOCRJOBTYPE_MMFILEHANDLE;

                IntPtr MMFhandle = IntPtr.Zero;

                imgStream.Seek(0, SeekOrigin.Begin);
                BMP = new Bitmap(imgStream);

                MMFhandle = ConvertBitmapToMMF(BMP, true, true);

                if (MMFhandle.Equals(IntPtr.Zero))
                    return Results;

                try
                {
                    TOCRSetConfig(TOCRCONFIG_DEFAULTJOB, TOCRCONFIG_DLL_ERRORMODE, TOCRERRORMODE_MSGBOX);

                    JobInfo2.hMMF = MMFhandle;

                    if (!OCRWait(JobNo, JobInfo2, ref error))
                        throw new TransymException("Unknown Transym Error. Engine did not respond. Transym error code: " + error);

                    if (!GetResults(JobNo, ref Results, ref error))
                        throw new TransymException("Transym Error. Could not get result. Transym error code: " + error);

                    if (error != 0)
                        throw new TransymException("Could not ocr stream. Transym error code: " + error);

                    return Results;

                }
                finally
                {
                    CloseHandle(MMFhandle);
                }
            }
            catch (TransymException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new TransymException("Unknown Transym Error", e);
            }
            finally
            {
                Stop();
                var task = Task.Factory.StartNew(() => { Stop(); });
                if (!task.Wait(5000))
                {
                    throw new TransymException("Timedout trying to process shutdown transym");
                }
            }
        } // Example4()


        //public TOcrResultStructures.TOcrResults OcrByBitmap(Bitmap BMP)
        //{
        //    try
        //    {
        //        int error = 0;
        //        int Status;
        //        int JobNo = 0;
        //        string Msg = "";
        //        TOcrResultStructures.TOcrResults Results = new TOcrResultStructures.TOcrResults();
        //        TOCRJOBINFO2 JobInfo2 = new TOCRJOBINFO2();

        //        JobInfo2.ProcessOptions.DisableCharacter = new short[256];
        //        JobInfo2.ProcessOptions.DeshadeOff = 1;
        //        JobInfo2.ProcessOptions.StructId = 0;

        //        //JobInfo2.ProcessOptions.InvertOff = 1;
        //        //JobInfo2.ProcessOptions.CharacterRejectOff = 1;
        //        IntPtr MMFhandle = IntPtr.Zero;

        //        //imgStream.Seek(0, SeekOrigin.Begin);
        //        //BMP = new Bitmap(imgStream);

        //        MMFhandle = ConvertBitmapToMMF(BMP, true, true);

        //        if (MMFhandle.Equals(IntPtr.Zero))
        //            return Results;

        //        try
        //        {
        //            TOCRSetConfig(TOCRCONFIG_DEFAULTJOB, TOCRCONFIG_DLL_ERRORMODE, TOCRERRORMODE_MSGBOX);
        //            JobInfo2.JobType = TOCRJOBTYPE_MMFILEHANDLE;

        //            Status = TOCRInitialise(ref JobNo);
        //            if (Status != TOCR_OK)
        //                throw new TransymException("Unknown Transym Error. Could not initialize engine");

        //            JobInfo2.hMMF = MMFhandle;
        //            try
        //            {
        //                if (!OCRWait(JobNo, JobInfo2, ref error))
        //                    throw new TransymException("Unknown Transym Error. Engine did not respond");

        //                if (!GetResults(JobNo, ref Results, ref error))
        //                    throw new TransymException("Transym Error. Could not get result");

        //                if (error != 0)
        //                    throw new TransymException("Could not ocr stream. Transym error code: " + error);

        //                return Results;
        //            }
        //            finally
        //            {
        //                TOCRShutdown(JobNo);
        //            }
        //        }
        //        finally
        //        {
        //            CloseHandle(MMFhandle);
        //        }
        //    }
        //    catch (TransymException)
        //    {
        //        throw;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new TransymException("Unknown Transym Error", e);
        //    }
        //} // Example4()

        // Convert a bitmap to a memory mapped file.
        // It does this by constructing a GDI bitmap in a byte array and copying this to a memory mapped file.
        private static IntPtr ConvertBitmapToMMF(Bitmap BMPIn, bool DiscardBitmap, bool ConvertTo1Bit)
        {
            Bitmap BMP;
            BITMAPINFOHEADER BIH;
            BitmapData BMPData;
            int ImageSize;
            byte[] Bytes;
            GCHandle BytesGC;
            int MMFsize;
            int PalEntries;
            RGBQUAD rgb;
            int Offset;
            IntPtr MMFhandle = IntPtr.Zero;
            IntPtr MMFview = IntPtr.Zero;
            IntPtr RetPtr = IntPtr.Zero;

            if (DiscardBitmap) // can destroy input bitmap
            {
                if (ConvertTo1Bit && BMPIn.PixelFormat != PixelFormat.Format1bppIndexed)
                {
                    BMP = ConvertTo1bpp(BMPIn);
                    BMPIn.Dispose();
                    BMPIn = null;
                }
                else
                {
                    BMP = BMPIn;
                }
            }
            else			  // must keep input bitmap unchanged 
            {
                if (ConvertTo1Bit && BMPIn.PixelFormat != PixelFormat.Format1bppIndexed)
                {
                    BMP = ConvertTo1bpp(BMPIn);
                }
                else
                {
                    BMP = BMPIn.Clone(new Rectangle(new Point(), BMPIn.Size), BMPIn.PixelFormat);
                }
            }

            // Flip the bitmap (GDI+ bitmap scan lines are top down, GDI are bottom up)
            BMP.RotateFlip(RotateFlipType.RotateNoneFlipY);
            BMPData = BMP.LockBits(new Rectangle(new Point(), BMP.Size), ImageLockMode.ReadOnly, BMP.PixelFormat);
            ImageSize = BMPData.Stride * BMP.Height;

            PalEntries = BMP.Palette.Entries.Length;

            BIH.biWidth = BMP.Width;
            BIH.biHeight = BMP.Height;
            BIH.biPlanes = 1;
            BIH.biClrImportant = 0;
            BIH.biCompression = BI_RGB;
            BIH.biSizeImage = (uint)ImageSize;
            BIH.biXPelsPerMeter = System.Convert.ToInt32(BMP.HorizontalResolution * 100 / 2.54);
            BIH.biYPelsPerMeter = System.Convert.ToInt32(BMP.VerticalResolution * 100 / 2.54);
            BIH.biBitCount = 0;	// to avoid "Use of unassigned local variable 'BIH'"
            BIH.biSize = 0;	// to avoid "Use of unassigned local variable 'BIH'"
            BIH.biClrImportant = 0;	// to avoid "Use of unassigned local variable 'BIH'"

            // Most of these formats are untested and the alpha channel is ignored
            switch (BMP.PixelFormat)
            {
                case PixelFormat.Format1bppIndexed:
                    BIH.biBitCount = 1;
                    break;
                case PixelFormat.Format4bppIndexed:
                    BIH.biBitCount = 4;
                    break;
                case PixelFormat.Format8bppIndexed:
                    BIH.biBitCount = 8;
                    break;
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format16bppGrayScale:
                case PixelFormat.Format16bppRgb555:
                case PixelFormat.Format16bppRgb565:
                    BIH.biBitCount = 16;
                    PalEntries = 0;
                    break;
                case PixelFormat.Format24bppRgb:
                    BIH.biBitCount = 24;
                    PalEntries = 0;
                    break;
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppRgb:
                    BIH.biBitCount = 32;
                    PalEntries = 0;
                    break;
            }
            BIH.biClrUsed = (uint)PalEntries;
            BIH.biSize = (uint)Marshal.SizeOf(BIH);

            MMFsize = Marshal.SizeOf(BIH) + PalEntries * Marshal.SizeOf(typeof(RGBQUAD)) + ImageSize;
            Bytes = new byte[MMFsize];

            BytesGC = GCHandle.Alloc(Bytes, GCHandleType.Pinned);
            Marshal.StructureToPtr(BIH, BytesGC.AddrOfPinnedObject(), true);
            Offset = Marshal.SizeOf(BIH);
            rgb.rgbReserved = 0;
            for (int PalEntry = 0; PalEntry <= PalEntries - 1; PalEntry++)
            {
                rgb.rgbRed = BMP.Palette.Entries[PalEntry].R;
                rgb.rgbGreen = BMP.Palette.Entries[PalEntry].G;
                rgb.rgbBlue = BMP.Palette.Entries[PalEntry].B;

                Marshal.StructureToPtr(rgb, Marshal.UnsafeAddrOfPinnedArrayElement(Bytes, Offset), false);
                Offset = Offset + Marshal.SizeOf(rgb);
            }
            BytesGC.Free();
            Marshal.Copy(BMPData.Scan0, Bytes, Offset, ImageSize);
            BMP.UnlockBits(BMPData);
            BMPData = null;
            BMP.Dispose();
            BMP = null;
            MMFhandle = CreateFileMappingMy(0xFFFFFFFF, 0, PAGE_READWRITE, 0, (uint)MMFsize, 0);
            if (!(MMFhandle.Equals(IntPtr.Zero)))
            {
                MMFview = MapViewOfFileMy(MMFhandle, FILE_MAP_WRITE, 0, 0, 0);
                if (MMFview.Equals(IntPtr.Zero))
                {
                    CloseHandle(MMFhandle);
                }
                else
                {
                    Marshal.Copy(Bytes, 0, MMFview, MMFsize);
                    UnmapViewOfFileMy(MMFview);
                    RetPtr = MMFhandle;
                }
            }

            Bytes = null;

            if (RetPtr.Equals(IntPtr.Zero))
            {
                //MessageBox.Show("Failed to convert bitmap", "ConvertBitmapToMMF", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            return RetPtr;
        }
        // Convert a bitmap to 1bpp
        private static Bitmap ConvertTo1bpp(Bitmap BMPIn)
        {

            BITMAPINFO bmi = new BITMAPINFO();
            IntPtr hbmIn = BMPIn.GetHbitmap();

            bmi.bmih.biSize = (uint)Marshal.SizeOf(bmi.bmih);
            bmi.bmih.biWidth = BMPIn.Width;
            bmi.bmih.biHeight = BMPIn.Height;
            bmi.bmih.biPlanes = 1;
            bmi.bmih.biBitCount = 1;
            bmi.bmih.biCompression = BI_RGB;
            bmi.bmih.biSizeImage = (uint)((((BMPIn.Width + 7) & 0xFFFFFFF8) >> 3) * BMPIn.Height);
            bmi.bmih.biXPelsPerMeter = System.Convert.ToInt32(BMPIn.HorizontalResolution * 100 / 2.54);
            bmi.bmih.biYPelsPerMeter = System.Convert.ToInt32(BMPIn.VerticalResolution * 100 / 2.54);
            bmi.bmih.biClrUsed = 2;
            bmi.bmih.biClrImportant = 2;
            bmi.cols = new uint[2]; // see the definition of BITMAPINFO()
            bmi.cols[0] = 0;
            bmi.cols[1] = ((uint)(255)) | ((uint)((255) << 8)) | ((uint)((255) << 16));

            IntPtr dummy;
            IntPtr hbm = CreateDIBSection(IntPtr.Zero, ref bmi, DIB_RGB_COLORS, out dummy, IntPtr.Zero, 0);

            IntPtr scrnDC = GetDC(IntPtr.Zero);
            IntPtr hDCIn = CreateCompatibleDC(scrnDC);

            SelectObject(hDCIn, hbmIn);
            IntPtr hDC = CreateCompatibleDC(scrnDC);
            SelectObject(hDC, hbm);

            BitBlt(hDC, 0, 0, BMPIn.Width, BMPIn.Height, hDCIn, 0, 0, SRCCOPY);

            Bitmap BMP = Bitmap.FromHbitmap(hbm);

            DeleteDC(hDCIn);
            DeleteDC(hDC);
            ReleaseDC(IntPtr.Zero, scrnDC);
            DeleteObject(hbmIn);
            DeleteObject(hbm);

            return BMP;
        }
    }
}
