using ImageMagick;
using System.Drawing.Imaging;
using System.Security;
using System.Windows.Forms;

namespace ReadMod
{
    public partial class Form1 : Form
    {
        public ImageInfo[] imgInfo;

        public Frame[] frame;

        public short[] arrFrame;

        public short[][] anim_data = new short[16][];

        public int ID;

        public int width;

        public int height;

        public int lengthImage = 0;
        public byte[] cacheImage;

        public List<ImageInfo> imageInfos;
        public List<Frame> frameInfos;
        public List<ImageCut> imageInfoCuts;

        private Point startPoint;

        private Rectangle selectedRegion;
        private bool isDragging = false;
        List<CutRegion> cutRegions = new List<CutRegion>();

        bool isMovingRegion = false;
        CutRegion selectedRegionBox = null;
        Point previousMouseLocation;

        public Form1()
        {
            var checkDate = DateTime.Now;

            if (checkDate <= new DateTime(2023, 7, 2))
            {
                InitializeComponent();
                imageInfos = new List<ImageInfo>();
                imageInfoCuts = new List<ImageCut>();
                frameInfos = new List<Frame>();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sbyte[] data = null;
            sbyte[] imageData = null;

            openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "D:\\Download\\mob\\x2";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedFileName = openFileDialog1.FileName;
                try
                {
                    using (StreamReader reader = new StreamReader(selectedFileName))
                    {
                        using (BinaryReader binaryReader = new BinaryReader(reader.BaseStream))
                        {
                            byte typeRead = binaryReader.ReadByte();

                            if (typeRead != 0)
                            {
                                int dataLength = ReadInt32(binaryReader);
                                data = new sbyte[dataLength];
                                for (int i = 0; i < dataLength; i++)
                                {
                                    data[i] = (sbyte)binaryReader.ReadByte();
                                }
                            }
                            else
                            {
                                int dataLength = ReadInt32(binaryReader);
                                data = new sbyte[dataLength];
                                for (int i = 0; i < dataLength; i++)
                                {
                                    data[i] = (sbyte)binaryReader.ReadByte();
                                }
                            }

                            int imageDataLength = ReadInt32(binaryReader);

                            imageData = new sbyte[imageDataLength];
                            for (int i = 0; i < imageDataLength; i++)
                            {
                                imageData[i] = (sbyte)binaryReader.ReadByte();
                            }
                        }
                    }

                    byte[] imageDataSave = new byte[imageData.Length];
                    Buffer.BlockCopy(imageData, 0, imageDataSave, 0, imageData.Length);

                    using (MemoryStream memoryStream = new MemoryStream(imageDataSave))
                    {
                        var outputPath = string.Format("{0}\\MobData\\x2\\", System.IO.Path.GetDirectoryName(Application.ExecutablePath));
                        Directory.CreateDirectory(outputPath);
                        string filePath = string.Format("{0}0.png", outputPath);

                        System.Drawing.Image i = System.Drawing.Image.FromStream(memoryStream);
                        try
                        {
                            SaveImage(i, outputPath, "0");
                        }
                        catch (Exception)
                        {

                        }

                        Bitmap bm = new Bitmap(memoryStream, false);
                        pictureBox1.Image = bm;
                        ReadDataMob(data, i, 2, 0);
                    }
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        private void SaveImageFromSByte(sbyte[] data, string outputPath)
        {
            byte[] imageData = new byte[data.Length];
            Buffer.BlockCopy(data, 0, imageData, 0, data.Length);

            using (MemoryStream memoryStream = new MemoryStream(imageData))
            {
                System.Drawing.Image i = System.Drawing.Image.FromStream(memoryStream);
                i.Save(outputPath, ImageFormat.Png);

                Bitmap bm = new Bitmap(memoryStream, false);
                pictureBox1.Image = bm;
                //ReadDataMob(imageData, i, 2, 0);
            }

        }

        public static short ReadShort(BinaryReader reader)
        {
            byte[] buffer = reader.ReadBytes(2);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(buffer);
            }
            return BitConverter.ToInt16(buffer, 0);
        }

        public static int ReadInt32(BinaryReader reader)
        {
            byte[] buffer = reader.ReadBytes(4);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(buffer);
            }
            return BitConverter.ToInt32(buffer, 0);
        }

        public static void ReadDataMob(sbyte[] data, System.Drawing.Image image, int zoomLevel, int mobId)
        {
            try
            {
                byte[] normalizeData = new byte[data.Length];
                Buffer.BlockCopy(data, 0, normalizeData, 0, data.Length);

                System.Drawing.Image[] imagesInfo = new System.Drawing.Image[256];

                using (MemoryStream stream = new MemoryStream(normalizeData))
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int nImageInfo = reader.ReadByte();
                    imagesInfo = new System.Drawing.Image[nImageInfo];

                    for (int i = 0; i < nImageInfo; i++)
                    {
                        int id = reader.ReadByte();
                        int x = reader.ReadByte() * zoomLevel;
                        int y = reader.ReadByte() * zoomLevel;
                        int w = reader.ReadByte() * zoomLevel;
                        int h = reader.ReadByte() * zoomLevel;

                        try
                        {
                            imagesInfo[id] = CropImage(image, x, y, w, h);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(x + " - " + y + " - " + w + " - " + h);
                            Console.WriteLine("loi doc mob: " + mobId);
                            Console.WriteLine(e.StackTrace);
                        }
                        var outputPath = string.Format("{0}/MobData/imginfo/x{1}/{2}", System.IO.Path.GetDirectoryName(Application.ExecutablePath), zoomLevel, mobId);
                        Directory.CreateDirectory(outputPath);

                        // Lưu từng bộ phận
                        SaveImage(imagesInfo[i], outputPath, $"{mobId}_{id}");
                    }

                    int nFrame = ReadShort(reader);
                    Bitmap[] frames = new Bitmap[nFrame];
                    for (int i = 0; i < nFrame; i++)
                    {
                        Bitmap frame = new Bitmap(400, 400, PixelFormat.Format32bppArgb);
                        Graphics g = Graphics.FromImage(frame);
                        byte nSubImage = reader.ReadByte();
                        for (int j = 0; j < nSubImage; j++)
                        {
                            int dx = ReadShort(reader) * zoomLevel;
                            int dy = ReadShort(reader) * zoomLevel;
                            byte imageId = reader.ReadByte();
                            g.DrawImage(imagesInfo[imageId], 100 + dx, 100 + dy);
                        }
                        frames[i] = frame;
                        // Save individual frame image

                        var outputPath = string.Format("{0}/MobData/frame/x{1}/{2}", System.IO.Path.GetDirectoryName(Application.ExecutablePath), zoomLevel, mobId);
                        Directory.CreateDirectory(outputPath);

                        SaveImage(TrimImage(frame), outputPath, $"{mobId}" + "_" + i + ".png");
                        //SaveImage(frame, "D:\\Mob\\read mob\\frame\\x" + zoomLevel + "\\" + mobId + "\\", $"{mobId}" + "_" + i + ".png");
                    }

                    int nAFrame = ReadShort(reader);

                    for (int i = 0; i < nAFrame; i++)
                    {
                        int frameId = ReadShort(reader);
                        // Console.WriteLine("frame id: " + frameId);
                    }

                    var clone = reader.ReadBytes(2);

                    //using (FileStream output = new FileStream($"D:\\Mob\\read mob\\framegif\\{mobId}.gif", FileMode.Create))
                    //{
                    // using (GifEncoder encoder = new GifEncoder(output))
                    // {
                    // encoder.Start();

                    // foreach (System.Drawing.Image frame in frames)
                    // {
                    // encoder.AddFrame(frame, TimeSpan.FromMilliseconds(300));
                    // }

                    // encoder.Finish();
                    // }
                    //}
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        private void btnBuildMob_Click(object sender, EventArgs e)
        {
            int i = 0;
            int zoom = !string.IsNullOrEmpty(lblZoom.Text) ? int.Parse(lblZoom.Text) : 2;

            List<MobPotitionInfo> mob = new List<MobPotitionInfo>();

            // Thêm type read và 4 byte là length của potion đằng sau
            for (i = 0; i < 5; i++)
            {
                mob.Add(new MobPotitionInfo()
                {
                    ID = i,
                    Value = 0
                });
            }

            // Thêm số lượng imgInfo
            mob.Add(new MobPotitionInfo()
            {
                ID = i,
                Value = (sbyte)imageInfos.Count
            });

            i++;

            // Thêm thông tin từng imgInfo
            foreach (var item in imageInfos)
            {
                mob.Add(new MobPotitionInfo()
                {
                    ID = i,
                    Value = (sbyte)item.ID
                });

                i++;

                mob.Add(new MobPotitionInfo()
                {
                    ID = i,
                    Value = (sbyte)(item.x / zoom)
                });

                i++;

                mob.Add(new MobPotitionInfo()
                {
                    ID = i,
                    Value = (sbyte)(item.y / zoom)
                });

                i++;

                mob.Add(new MobPotitionInfo()
                {
                    ID = i,
                    Value = (sbyte)(item.w / zoom)
                });

                i++;

                mob.Add(new MobPotitionInfo()
                {
                    ID = i,
                    Value = (sbyte)(item.h / zoom)
                });

                i++;
            }

            // Thêm số lượng frame
            var frameLength = ShortToBytes((short)frameInfos.Count);

            foreach (var item in frameLength)
            {
                mob.Add(new MobPotitionInfo()
                {
                    ID = i,
                    Value = (sbyte)item
                });

                i++;
            }

            foreach (var item in frameInfos)
            {
                mob.Add(new MobPotitionInfo()
                {
                    ID = i,
                    Value = (sbyte)item.nSubImage
                });

                i++;

                for (int j = 0; j < item.nSubImage; j++)
                {
                    var dx = ShortToSBytes((short)(item.dx[j] / zoom));
                    var dy = ShortToSBytes((short)(item.dy[j] / zoom));

                    foreach (var dxItem in dx)
                    {
                        mob.Add(new MobPotitionInfo()
                        {
                            ID = i,
                            Value = dxItem
                        });

                        i++;
                    }

                    foreach (var dyItem in dy)
                    {
                        mob.Add(new MobPotitionInfo()
                        {
                            ID = i,
                            Value = dyItem
                        });

                        i++;
                    }

                    mob.Add(new MobPotitionInfo()
                    {
                        ID = i,
                        Value = (sbyte)item.idImg[j]
                    });

                    i++;
                }
            }

            var nAFrame = ShortToSBytes((short)frameInfos.Count);

            foreach (var item in nAFrame)
            {
                mob.Add(new MobPotitionInfo()
                {
                    ID = i,
                    Value = item
                });

                i++;
            }

            for (int m = 0; m < frameInfos.Count; m++)
            {
                var frameId = ShortToSBytes((short)m);

                foreach (var item in frameId)
                {
                    mob.Add(new MobPotitionInfo()
                    {
                        ID = i,
                        Value = item
                    });

                    i++;
                }
            }

            mob.Add(new MobPotitionInfo()
            {
                ID = i,
                Value = 0
            });

            i++;

            mob.Add(new MobPotitionInfo()
            {
                ID = i,
                Value = 0
            });

            i++;

            var dataLength = IntToSBytes(mob.Count - 5);

            for (int k = 0; k < dataLength.Length; k++)
            {
                mob[k + 1].Value = dataLength[k];
            }

            var lengthImageBytes = IntToBytes(lengthImage);

            using (BinaryWriter writer = new BinaryWriter(File.Open("mob_build", FileMode.Create)))
            {
                foreach (var item in mob)
                {
                    writer.Write(item.Value);
                }
                writer.Write(lengthImageBytes);
                writer.Write(cacheImage);
            }
        }

        public sbyte[] ShortToSBytes(short value)
        {
            sbyte[] sbytes = new sbyte[2];
            sbytes[0] = (sbyte)(value >> 8);   // Lấy sbyte thứ nhất từ giá trị short
            sbytes[1] = (sbyte)value;           // Lấy sbyte thứ hai từ giá trị short
            return sbytes;
        }

        public sbyte[] IntToSBytes(int value)
        {
            sbyte[] sbytes = new sbyte[4];
            sbytes[0] = (sbyte)(value >> 24);      // Lấy sbyte thứ nhất từ giá trị int
            sbytes[1] = (sbyte)(value >> 16);      // Lấy sbyte thứ hai từ giá trị int
            sbytes[2] = (sbyte)(value >> 8);       // Lấy sbyte thứ ba từ giá trị int
            sbytes[3] = (sbyte)value;               // Lấy sbyte thứ tư từ giá trị int
            return sbytes;
        }

        public byte[] ShortToBytes(short value)
        {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)(value >> 8);       // Lấy byte thứ nhất từ giá trị short
            bytes[1] = (byte)(value & 0xFF);     // Lấy byte thứ hai từ giá trị short
            return bytes;
        }

        public byte[] IntToBytes(int value)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(value >> 24);      // Lấy byte thứ nhất từ giá trị int
            bytes[1] = (byte)(value >> 16);      // Lấy byte thứ hai từ giá trị int
            bytes[2] = (byte)(value >> 8);       // Lấy byte thứ ba từ giá trị int
            bytes[3] = (byte)(value & 0xFF);     // Lấy byte thứ tư từ giá trị int
            return bytes;
        }

        public static System.Drawing.Image CropImage(System.Drawing.Image image, int x, int y, int width, int height)
        {
            Rectangle cropRect = new Rectangle(x, y, width, height);
            Bitmap croppedBitmap = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(croppedBitmap))
            {
                g.DrawImage(image, 0, 0, cropRect, GraphicsUnit.Pixel);
            }

            return croppedBitmap;
        }

        public static Bitmap TrimImage(Bitmap image)
        {
            Rectangle bounds = GetTrimBounds(image);

            Bitmap trimmedImage = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics graphics = Graphics.FromImage(trimmedImage))
            {
                graphics.DrawImage(image, 0, 0, bounds, GraphicsUnit.Pixel);
            }

            return trimmedImage;
        }

        private static Rectangle GetTrimBounds(Bitmap image)
        {
            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            IntPtr scan0 = bitmapData.Scan0;

            int left = image.Width;
            int top = image.Height;
            int right = 0;
            int bottom = 0;

            unsafe
            {
                byte* ptr = (byte*)scan0;

                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        byte alpha = ptr[3];

                        if (alpha != 0)
                        {
                            if (x < left)
                                left = x;
                            if (x > right)
                                right = x;
                            if (y < top)
                                top = y;
                            if (y > bottom)
                                bottom = y;
                        }

                        ptr += 4;
                    }

                    ptr += bitmapData.Stride - (image.Width * 4);
                }
            }

            image.UnlockBits(bitmapData);

            int width = right - left + 1;
            int height = bottom - top + 1;

            return new Rectangle(left, top, width, height);
        }

        public static void SaveImage(System.Drawing.Image image, string outputPath, string fileName)
        {
            Directory.CreateDirectory(outputPath);
            string filePath = Path.Combine(outputPath, fileName + ".png");
            image.Save(filePath, ImageFormat.Png);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string[] fileNames = openFileDialog.FileNames;

                if (fileNames.Length >= 2)
                {
                    string outputFile = "output.bin";
                    MergeBinaryFiles(fileNames[0], fileNames[1], outputFile);
                    MessageBox.Show("Đã hoàn thành việc gộp file.");
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn ít nhất hai file để gộp.");
                }
            }

        }

        private void MergeBinaryFiles(string inputFile1, string inputFile2, string outputFile)
        {
            byte[] frames, pngs;

            using (BinaryReader reader1 = new BinaryReader(File.Open(inputFile1, FileMode.Open)))
            {
                frames = new byte[reader1.BaseStream.Length - 4];

                for (int i = 0; i < reader1.BaseStream.Length - 4; i++)
                {
                    frames[i] = reader1.ReadByte();
                }
            }

            using (BinaryReader reader2 = new BinaryReader(File.Open(inputFile2, FileMode.Open)))
            {
                pngs = new byte[reader2.BaseStream.Length + 4];

                byte[] byteValue = ConvertIntToBytes((int)reader2.BaseStream.Length);

                for (int i = 0; i < reader2.BaseStream.Length + 4; i++)
                {
                    if (i < 4)
                    {
                        pngs[i] = byteValue[i];
                    }
                    else
                    {
                        pngs[i] = (byte)reader2.ReadByte();
                    }
                }
            }

            using (BinaryWriter writer = new BinaryWriter(File.Open(outputFile, FileMode.Create)))
            {
                writer.Write(frames);
                writer.Write(pngs);
            }
        }

        public static byte[] ConvertIntToBytes(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes); // Đảo ngược thứ tự byte nếu máy tính sử dụng định dạng Little Endian

            return bytes;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            startPoint = e.Location;
            isDragging = true;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            int width = e.X - startPoint.X;
            int height = e.Y - startPoint.Y;

            // Tọa độ x, y của điểm bắt đầu
            int startX = startPoint.X;
            int startY = startPoint.Y;

            lblX.Text = startX.ToString();
            lblY.Text = startY.ToString();
            lblHeight.Text = height.ToString();
            lblWidth.Text = width.ToString();

            // Sử dụng startX, startY, width và height trong các xử lý tiếp theo của bạn
            isDragging = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int width = e.X - startPoint.X;
                int height = e.Y - startPoint.Y;

                selectedRegion = new Rectangle(startPoint.X, startPoint.Y, width, height);
                pictureBox1.Invalidate(); // Gọi lại phương thức Paint để vẽ lại PictureBox
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (selectedRegion != null)
            {
                e.Graphics.DrawRectangle(Pens.Red, selectedRegion);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Red)), selectedRegion);
            }
        }

        private void SaveRegionAsImage(Bitmap originalImage, int startX, int startY, int width, int height, string outputPath)
        {
            // Tạo một bitmap mới để chứa region
            Bitmap regionImage = new Bitmap(width, height);

            // Lặp qua từng pixel của region và sao chép từ ảnh gốc vào ảnh region
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixelColor = originalImage.GetPixel(startX + x, startY + y);
                    regionImage.SetPixel(x, y, pixelColor);
                }
            }

            // Lưu ảnh region vào file
            regionImage.Save(outputPath);

            // Giải phóng tài nguyên
            regionImage.Dispose();
        }

        private void RedrawRegionSelect()
        {
            int x = int.Parse(!string.IsNullOrEmpty(lblX.Text) ? lblX.Text : "0");
            int y = int.Parse(!string.IsNullOrEmpty(lblY.Text) ? lblY.Text : "0");
            int width = int.Parse(!string.IsNullOrEmpty(lblWidth.Text) ? lblWidth.Text : "0");
            int height = int.Parse(!string.IsNullOrEmpty(lblHeight.Text) ? lblHeight.Text : "0");

            selectedRegion = new Rectangle(x, y, width, height);
            pictureBox1.Invalidate(); // Gọi lại phương thức Paint để vẽ lại PictureBox
        }

        private void btnRedraw_Click(object sender, EventArgs e)
        {
            RedrawRegionSelect();
        }

        private void IncreaseValue(TextBox textBox)
        {
            int value;
            if (int.TryParse(textBox.Text, out value))
            {
                value++;
                textBox.Text = value.ToString();
                UpdateRegion();
            }
        }

        private void DecreaseValue(TextBox textBox)
        {
            int value;
            if (int.TryParse(textBox.Text, out value))
            {
                if (value > 0)
                {
                    value--;
                    textBox.Text = value.ToString();
                    UpdateRegion();
                }
            }
        }

        private void UpdateRegion()
        {
            int id, x, y, width, height;
            GetImageInfos(out id, out x, out y, out width, out height);

            selectedRegion = new Rectangle(x, y, width, height);
            pictureBox1.Invalidate(); // Gọi lại phương thức Paint để vẽ lại PictureBox
        }

        private void GetImageInfos(out int id, out int x, out int y, out int width, out int height)
        {
            try
            {
                id = int.Parse(lblID.Text);
            }
            catch (Exception)
            {
                id = 0;
            }
            x = int.Parse(lblX.Text);
            y = int.Parse(lblY.Text);
            width = int.Parse(lblWidth.Text);
            height = int.Parse(lblHeight.Text);
        }

        private void lblWidth_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                IncreaseValue(lblWidth);
            }
            else if (e.KeyCode == Keys.Down)
            {
                DecreaseValue(lblWidth);
            }
        }

        private void lblHeight_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                IncreaseValue(lblHeight);
            }
            else if (e.KeyCode == Keys.Down)
            {
                DecreaseValue(lblHeight);
            }
        }

        private void lblX_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                IncreaseValue(lblX);
            }
            else if (e.KeyCode == Keys.Down)
            {
                DecreaseValue(lblX);
            }
        }

        private void lblY_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                IncreaseValue(lblY);
            }
            else if (e.KeyCode == Keys.Down)
            {
                DecreaseValue(lblY);
            }
        }

        private void btnSaveRegion_Click(object sender, EventArgs e)
        {
            int id, x, y, width, height;
            GetImageInfos(out id, out x, out y, out width, out height);

            var image = SaveImageRegion(x, y, width, height);
            SynImageInfos(id, x, y, width, height);
            var imageCut = imageInfoCuts.FirstOrDefault(n => n.ID == id);

            if (imageCut != null)
            {
                imageCut.Image = image;
            }
            else
            {
                imageInfoCuts.Add(new ImageCut()
                {
                    ID = id,
                    Image = image,
                });
            }
        }

        private void SynImageInfos(int id, int x, int y, int width, int height)
        {
            var imgInfo = imageInfos.FirstOrDefault(n => n.ID == id);

            if (imgInfo != null)
            {
                imgInfo.x = x;
                imgInfo.y = y;
                imgInfo.w = width;
                imgInfo.h = height;
            }
            else
            {
                imageInfos.Add(new ImageInfo()
                {
                    ID = id,
                    x = x,
                    y = y,
                    w = width,
                    h = height,
                });
            }
        }

        private Image SaveImageRegion(int x, int y, int width, int height)
        {
            Rectangle selectedRegion = new Rectangle(x, y, width, height); // Ví dụ: Hình chữ nhật vùng chọn

            // Tạo đối tượng Bitmap với kích thước của vùng chọn
            Bitmap bitmap = new Bitmap(selectedRegion.Width, selectedRegion.Height);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // Vẽ nội dung của vùng chọn lên đối tượng Bitmap
                graphics.DrawImage(pictureBox1.Image, new Rectangle(0, 0, selectedRegion.Width, selectedRegion.Height),
                    selectedRegion, GraphicsUnit.Pixel);
            }

            var outputPath = string.Format("{0}/Mob/imageInfos", System.IO.Path.GetDirectoryName(Application.ExecutablePath));
            Directory.CreateDirectory(outputPath);
            string filePath = Path.Combine(outputPath, lblID.Text + ".png");

            // Lưu đối tượng Bitmap thành file ảnh
            bitmap.Save(filePath, ImageFormat.Png);

            return bitmap;
        }

        private void btnClearRegion_Click(object sender, EventArgs e)
        {
            selectedRegion = new Rectangle();

            pictureBox1.Invalidate(); // Gọi lại phương thức Paint để vẽ lại PictureBox
        }

        private void btnAddToFrame_Click(object sender, EventArgs e)
        {
            int id, x, y, width, height;
            GetImageInfos(out id, out x, out y, out width, out height);

            // Lấy thông tin vùng cắt từ danh sách
            CutRegion selectedRegion = cutRegions.FirstOrDefault(r => r.ID == id);

            if (selectedRegion == null)
            {
                selectedRegion = new CutRegion()
                {
                    ID = id,
                    Bounds = new Rectangle(x, y, width, height),
                    Image = imageInfoCuts.FirstOrDefault(n => n.ID == id).Image
                };

                cutRegions.Add(selectedRegion);

                lblNumSub.Text = cutRegions.Count.ToString();
            }

            // Vẽ vùng cắt lên PictureBox 2
            using (Graphics g = pictureBox2.CreateGraphics())
            {
                g.DrawImage(selectedRegion.Image, selectedRegion.Bounds);
            }
            pictureBox2.Invalidate();
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            // Vẽ các vùng cắt trên PictureBox 2
            //foreach (CutRegion cutRegion in cutRegions)
            //{
            // e.Graphics.DrawImage(cutRegion.Image, cutRegion.Bounds);
            //}

            // Tính toán tọa độ giữa của PictureBox2
            int centerX = pictureBox2.Width / 2;
            int centerY = pictureBox2.Height / 2;

            // Duyệt qua danh sách CutRegion và điều chỉnh tọa độ
            foreach (var region in cutRegions)
            {
                // Tính toán lại tọa độ của region
                int adjustedX = region.Bounds.X + centerX;
                int adjustedY = region.Bounds.Y + centerY;
                Rectangle adjustedRegion = new Rectangle(adjustedX, adjustedY, region.Bounds.Width, region.Bounds.Height);

                // Vẽ region đã tính toán lại tọa độ lên PictureBox2
                e.Graphics.DrawImage(region.Image, adjustedRegion);
            }

            // Vẽ trục tọa độ
            e.Graphics.DrawLine(Pens.Black, centerX, 0, centerX, pictureBox2.Height);
            e.Graphics.DrawLine(Pens.Black, 0, centerY, pictureBox2.Width, centerY);
            e.Graphics.DrawString("(0, 0)", this.Font, Brushes.Black, new PointF(centerX + 5, centerY + 5));
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            Rectangle regionCheck;
            // Kiểm tra xem chuột có nằm trên một vùng cắt hay không
            foreach (CutRegion cutRegion in cutRegions)
            {
                regionCheck = new Rectangle(cutRegion.Bounds.X + pictureBox2.Width / 2, cutRegion.Bounds.Y + pictureBox2.Height / 2, cutRegion.Bounds.Width, cutRegion.Bounds.Height);
                if (regionCheck.Contains(e.Location))
                {
                    isMovingRegion = true;
                    selectedRegionBox = cutRegion;
                    previousMouseLocation = e.Location;
                    UpdatePotionFrame(cutRegion);
                    break;
                }
            }
        }

        private void UpdatePotionFrame(CutRegion cutRegion)
        {
            lblImageId.Text = cutRegion.ID.ToString();
            lblDX.Text = cutRegion.Bounds.X.ToString();
            lblDY.Text = cutRegion.Bounds.Y.ToString();
        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMovingRegion && selectedRegionBox != null)
            {
                // Tạo một bản sao của Bounds
                Rectangle newBounds = selectedRegionBox.Bounds;

                // Tính toán khoảng cách di chuyển của chuột
                int deltaX = e.Location.X - previousMouseLocation.X;
                int deltaY = e.Location.Y - previousMouseLocation.Y;

                // Di chuyển vùng cắt dựa trên khoảng cách di chuyển của chuột
                newBounds.X += deltaX;
                newBounds.Y += deltaY;

                // Gán bản sao vào Bounds của selectedRegion
                selectedRegionBox.Bounds = newBounds;

                // Lưu vị trí chuột hiện tại cho lần di chuyển tiếp theo
                previousMouseLocation = e.Location;

                // Vẽ lại PictureBox 2
                pictureBox2.Invalidate();

                UpdatePotionFrame(selectedRegionBox);
            }
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            isMovingRegion = false;
            selectedRegionBox = null;
        }

        private void lblDX_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                IncreaseValueBox(lblDX);
            }
            else if (e.KeyCode == Keys.Down)
            {
                DecreaseValueBox(lblDX);
            }
        }

        private void lblDY_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                IncreaseValueBox(lblDY);
            }
            else if (e.KeyCode == Keys.Down)
            {
                DecreaseValueBox(lblDY);
            }
        }

        private void IncreaseValueBox(TextBox textBox)
        {
            int value;
            if (int.TryParse(textBox.Text, out value))
            {
                value++;
                textBox.Text = value.ToString();
                UpdateRegionBox();
            }
        }

        private void DecreaseValueBox(TextBox textBox)
        {
            int value;
            if (int.TryParse(textBox.Text, out value))
            {
                value--;
                textBox.Text = value.ToString();
                UpdateRegionBox();
            }
        }

        private void UpdateRegionBox()
        {
            // Lấy ID, X và Y từ TextBox
            int regionId = int.Parse(lblImageId.Text);
            int newX = int.Parse(lblDX.Text);
            int newY = int.Parse(lblDY.Text);

            // Lấy region từ danh sách CutRegion dựa trên ID
            var region = cutRegions.FirstOrDefault(r => r.ID == regionId);

            if (region != null)
            {
                // Tạo một bản sao của Bounds
                Rectangle newBounds = region.Bounds;

                // Di chuyển vùng cắt dựa trên khoảng cách di chuyển của chuột
                newBounds.X = newX;
                newBounds.Y = newY;

                // Gán bản sao vào Bounds của selectedRegion
                region.Bounds = newBounds;

                // Vẽ lại PictureBox2 để hiển thị region với tọa độ mới
                pictureBox2.Invalidate();
            }
        }

        private void pictureBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Rectangle regionCheck;

            // Xác định tọa độ double-click
            Point clickPoint = e.Location;

            // Lặp qua danh sách CutRegion và kiểm tra tọa độ double-click
            for (int i = cutRegions.Count - 1; i >= 0; i--)
            {
                var region = cutRegions[i];

                regionCheck = new Rectangle(region.Bounds.X + pictureBox2.Width / 2, region.Bounds.Y + pictureBox2.Height / 2, region.Bounds.Width, region.Bounds.Height);

                // Kiểm tra tọa độ double-click nằm trong vùng của region
                if (regionCheck.Contains(clickPoint))
                {
                    // Xóa region khỏi danh sách CutRegion
                    cutRegions.RemoveAt(i);
                }
            }

            // Vẽ lại PictureBox2 để hiển thị hình ảnh mới
            pictureBox2.Invalidate();

            lblNumSub.Text = cutRegions.Count.ToString();
        }

        private void btnSaveFrame_Click(object sender, EventArgs e)
        {
            var frameInfo = frameInfos.FirstOrDefault(n => n.ID.ToString() == lblFrameID.Text);

            var updateFrame = new Frame();

            updateFrame.nSubImage = (byte)cutRegions.Count;
            updateFrame.ID = !string.IsNullOrEmpty(lblFrameID.Text) ? int.Parse(lblFrameID.Text) : 0;
            updateFrame.dx = new short[cutRegions.Count];
            updateFrame.dy = new short[cutRegions.Count];
            updateFrame.idImg = new sbyte[cutRegions.Count];

            for (int i = 0; i < cutRegions.Count; i++)
            {
                updateFrame.dx[i] = (short)cutRegions[i].Bounds.X;
                updateFrame.dy[i] = (short)cutRegions[i].Bounds.Y;
                updateFrame.idImg[i] = (sbyte)cutRegions[i].ID;
            }

            if (frameInfo != null)
            {
                frameInfo.nSubImage = updateFrame.nSubImage;
                frameInfo.dx = updateFrame.dx;
                frameInfo.dy = updateFrame.dy;
                frameInfo.idImg = updateFrame.idImg;
            }
            else
            {
                frameInfos.Add(updateFrame);
            }

            Bitmap frame = new Bitmap(400, 400, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(frame);
            byte nSubImage = updateFrame.nSubImage;
            for (int j = 0; j < nSubImage; j++)
            {
                int dx = updateFrame.dx[j];
                int dy = updateFrame.dy[j];
                byte imageId = (byte)updateFrame.idImg[j];
                g.DrawImage(cutRegions.FirstOrDefault(n => n.ID == imageId).Image, 100 + dx, 100 + dy);
            }

            var outputPath = string.Format("{0}/Mob/frame", System.IO.Path.GetDirectoryName(Application.ExecutablePath));
            Directory.CreateDirectory(outputPath);

            // Save individual frame image
            SaveImage(TrimImage(frame), outputPath, lblFrameID.Text);
        }

        private void btnSelectImage_Click(object sender, EventArgs e)
        {
            openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string imagePath = openFileDialog1.FileName;

                // Đọc các byte từ tệp ảnh
                byte[] imageBytes = File.ReadAllBytes(imagePath);

                lengthImage = imageBytes.Length;

                cacheImage = imageBytes;

                // Tạo một MemoryStream từ byte array
                using (MemoryStream memoryStream = new MemoryStream(imageBytes))
                {
                    // Tạo một đối tượng Bitmap từ MemoryStream
                    Bitmap bitmap = new Bitmap(memoryStream);

                    // Hiển thị ảnh trên PictureBox
                    pictureBox1.Image = bitmap;
                }
            }
        }

        private void btnClearMob_Click(object sender, EventArgs e)
        {
            imageInfos = new List<ImageInfo>();
            imageInfoCuts = new List<ImageCut>();
            frameInfos = new List<Frame>();
        }
    }
}