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

        public List<ImageInfo> imageInfos;
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
            InitializeComponent();
            imageInfos = new List<ImageInfo>();
            imageInfoCuts = new List<ImageCut>();
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
                        System.Drawing.Image i = System.Drawing.Image.FromStream(memoryStream);
                        i.Save("C:\\Users\\ADMIN-PC\\Desktop\\New folder (6)\\mob\\x2\\0.png", ImageFormat.Png);

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

                        // Lưu từng bộ phận
                        SaveImage(imagesInfo[i], $"C:\\Users\\ADMIN-PC\\Desktop\\New folder (6)\\mob\\imginfo\\x{zoomLevel}\\{mobId}", $"{mobId}_{id}");
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
                        SaveImage(TrimImage(frame), "C:\\Users\\ADMIN-PC\\Desktop\\New folder (6)\\mob\\frame\\x" + zoomLevel + "\\" + mobId + "\\", $"{mobId}" + "_" + i + ".png");
                        //SaveImage(frame, "D:\\Mob\\read mob\\frame\\x" + zoomLevel + "\\" + mobId + "\\", $"{mobId}" + "_" + i + ".png");
                    }

                    //int nAFrame = reader.ReadInt16();

                    //for (int i = 0; i < nAFrame; i++)
                    //{
                    // int frameId = reader.ReadInt16();
                    // // Console.WriteLine("frame id: " + frameId);
                    //}

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

        }

        private void lblY_KeyDown(object sender, KeyEventArgs e)
        {

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
            }

            // Vẽ vùng cắt lên PictureBox 2
            using (Graphics g = pictureBox2.CreateGraphics())
            {
                g.DrawImage(selectedRegion.Image, selectedRegion.Bounds);
            }
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            // Vẽ các vùng cắt trên PictureBox 2
            foreach (CutRegion cutRegion in cutRegions)
            {
                e.Graphics.DrawImage(cutRegion.Image, cutRegion.Bounds);
            }
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            // Kiểm tra xem chuột có nằm trên một vùng cắt hay không
            foreach (CutRegion cutRegion in cutRegions)
            {
                if (cutRegion.Bounds.Contains(e.Location))
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
                if (value > 0)
                {
                    value--;
                    textBox.Text = value.ToString();
                    UpdateRegionBox();
                }
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
    }
}