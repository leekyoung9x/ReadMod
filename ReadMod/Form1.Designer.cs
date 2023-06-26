namespace ReadMod
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            openFileDialog1 = new OpenFileDialog();
            button1 = new Button();
            pictureBox1 = new PictureBox();
            button2 = new Button();
            label1 = new Label();
            lblX = new TextBox();
            lblY = new TextBox();
            label2 = new Label();
            lblWidth = new TextBox();
            label3 = new Label();
            lblHeight = new TextBox();
            label4 = new Label();
            lblID = new TextBox();
            label5 = new Label();
            btnRedraw = new Button();
            btnSaveRegion = new Button();
            btnClearRegion = new Button();
            btnAddToFrame = new Button();
            pictureBox2 = new PictureBox();
            label6 = new Label();
            lblImageId = new TextBox();
            lblDY = new TextBox();
            label7 = new Label();
            lblDX = new TextBox();
            label8 = new Label();
            label9 = new Label();
            lblFrameID = new TextBox();
            lblNumSub = new TextBox();
            label10 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // button1
            // 
            button1.Location = new Point(154, 12);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "Chọn";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(12, 69);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(356, 221);
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            pictureBox1.Paint += pictureBox1_Paint;
            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.MouseUp += pictureBox1_MouseUp;
            // 
            // button2
            // 
            button2.Location = new Point(12, 12);
            button2.Name = "button2";
            button2.Size = new Size(136, 23);
            button2.TabIndex = 2;
            button2.Text = "Chuyển về file mob";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(503, 101);
            label1.Name = "label1";
            label1.Size = new Size(16, 15);
            label1.TabIndex = 3;
            label1.Text = "x:";
            // 
            // lblX
            // 
            lblX.Location = new Point(565, 98);
            lblX.Name = "lblX";
            lblX.Size = new Size(100, 23);
            lblX.TabIndex = 4;
            lblX.KeyDown += lblX_KeyDown;
            // 
            // lblY
            // 
            lblY.Location = new Point(565, 127);
            lblY.Name = "lblY";
            lblY.Size = new Size(100, 23);
            lblY.TabIndex = 6;
            lblY.KeyDown += lblY_KeyDown;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(503, 130);
            label2.Name = "label2";
            label2.Size = new Size(16, 15);
            label2.TabIndex = 5;
            label2.Text = "y:";
            // 
            // lblWidth
            // 
            lblWidth.Location = new Point(565, 156);
            lblWidth.Name = "lblWidth";
            lblWidth.Size = new Size(100, 23);
            lblWidth.TabIndex = 8;
            lblWidth.KeyDown += lblWidth_KeyDown;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(503, 159);
            label3.Name = "label3";
            label3.Size = new Size(42, 15);
            label3.TabIndex = 7;
            label3.Text = "Width:";
            // 
            // lblHeight
            // 
            lblHeight.Location = new Point(565, 185);
            lblHeight.Name = "lblHeight";
            lblHeight.Size = new Size(100, 23);
            lblHeight.TabIndex = 10;
            lblHeight.KeyDown += lblHeight_KeyDown;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(503, 188);
            label4.Name = "label4";
            label4.Size = new Size(46, 15);
            label4.TabIndex = 9;
            label4.Text = "Height:";
            // 
            // lblID
            // 
            lblID.Location = new Point(565, 69);
            lblID.Name = "lblID";
            lblID.Size = new Size(100, 23);
            lblID.TabIndex = 12;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(503, 77);
            label5.Name = "label5";
            label5.Size = new Size(21, 15);
            label5.TabIndex = 13;
            label5.Text = "ID:";
            // 
            // btnRedraw
            // 
            btnRedraw.Location = new Point(683, 98);
            btnRedraw.Name = "btnRedraw";
            btnRedraw.Size = new Size(75, 23);
            btnRedraw.TabIndex = 14;
            btnRedraw.Text = "Khoanh lại";
            btnRedraw.UseVisualStyleBackColor = true;
            btnRedraw.Click += btnRedraw_Click;
            // 
            // btnSaveRegion
            // 
            btnSaveRegion.Location = new Point(683, 127);
            btnSaveRegion.Name = "btnSaveRegion";
            btnSaveRegion.Size = new Size(75, 23);
            btnSaveRegion.TabIndex = 15;
            btnSaveRegion.Text = "Lưu vùng";
            btnSaveRegion.UseVisualStyleBackColor = true;
            btnSaveRegion.Click += btnSaveRegion_Click;
            // 
            // btnClearRegion
            // 
            btnClearRegion.Location = new Point(683, 155);
            btnClearRegion.Name = "btnClearRegion";
            btnClearRegion.Size = new Size(75, 23);
            btnClearRegion.TabIndex = 16;
            btnClearRegion.Text = "Clear vùng";
            btnClearRegion.UseVisualStyleBackColor = true;
            btnClearRegion.Click += btnClearRegion_Click;
            // 
            // btnAddToFrame
            // 
            btnAddToFrame.Location = new Point(683, 184);
            btnAddToFrame.Name = "btnAddToFrame";
            btnAddToFrame.Size = new Size(75, 23);
            btnAddToFrame.TabIndex = 17;
            btnAddToFrame.Text = "Thêm vào frame";
            btnAddToFrame.UseVisualStyleBackColor = true;
            btnAddToFrame.Click += btnAddToFrame_Click;
            // 
            // pictureBox2
            // 
            pictureBox2.Location = new Point(12, 337);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(356, 323);
            pictureBox2.TabIndex = 18;
            pictureBox2.TabStop = false;
            pictureBox2.Paint += pictureBox2_Paint;
            pictureBox2.MouseDown += pictureBox2_MouseDown;
            pictureBox2.MouseMove += pictureBox2_MouseMove;
            pictureBox2.MouseUp += pictureBox2_MouseUp;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(505, 380);
            label6.Name = "label6";
            label6.Size = new Size(21, 15);
            label6.TabIndex = 24;
            label6.Text = "ID:";
            // 
            // lblImageId
            // 
            lblImageId.Location = new Point(565, 377);
            lblImageId.Name = "lblImageId";
            lblImageId.Size = new Size(100, 23);
            lblImageId.TabIndex = 23;
            // 
            // lblDY
            // 
            lblDY.Location = new Point(565, 435);
            lblDY.Name = "lblDY";
            lblDY.Size = new Size(100, 23);
            lblDY.TabIndex = 22;
            lblDY.KeyDown += lblDY_KeyDown;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(503, 438);
            label7.Name = "label7";
            label7.Size = new Size(23, 15);
            label7.TabIndex = 21;
            label7.Text = "dy:";
            // 
            // lblDX
            // 
            lblDX.Location = new Point(565, 406);
            lblDX.Name = "lblDX";
            lblDX.Size = new Size(100, 23);
            lblDX.TabIndex = 20;
            lblDX.KeyDown += lblDX_KeyDown;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(503, 409);
            label8.Name = "label8";
            label8.Size = new Size(23, 15);
            label8.TabIndex = 19;
            label8.Text = "dx:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(503, 356);
            label9.Name = "label9";
            label9.Size = new Size(54, 15);
            label9.TabIndex = 26;
            label9.Text = "FrameID:";
            // 
            // lblFrameID
            // 
            lblFrameID.Location = new Point(565, 348);
            lblFrameID.Name = "lblFrameID";
            lblFrameID.Size = new Size(100, 23);
            lblFrameID.TabIndex = 25;
            // 
            // lblNumSub
            // 
            lblNumSub.Location = new Point(565, 464);
            lblNumSub.Name = "lblNumSub";
            lblNumSub.Size = new Size(100, 23);
            lblNumSub.TabIndex = 28;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(503, 467);
            label10.Name = "label10";
            label10.Size = new Size(37, 15);
            label10.TabIndex = 27;
            label10.Text = "nSub:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 672);
            Controls.Add(lblNumSub);
            Controls.Add(label10);
            Controls.Add(label9);
            Controls.Add(lblFrameID);
            Controls.Add(label6);
            Controls.Add(lblImageId);
            Controls.Add(lblDY);
            Controls.Add(label7);
            Controls.Add(lblDX);
            Controls.Add(label8);
            Controls.Add(pictureBox2);
            Controls.Add(btnAddToFrame);
            Controls.Add(btnClearRegion);
            Controls.Add(btnSaveRegion);
            Controls.Add(btnRedraw);
            Controls.Add(label5);
            Controls.Add(lblID);
            Controls.Add(lblHeight);
            Controls.Add(label4);
            Controls.Add(lblWidth);
            Controls.Add(label3);
            Controls.Add(lblY);
            Controls.Add(label2);
            Controls.Add(lblX);
            Controls.Add(label1);
            Controls.Add(button2);
            Controls.Add(pictureBox1);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Mob";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private OpenFileDialog openFileDialog1;
        private Button button1;
        private PictureBox pictureBox1;
        private Button button2;
        private Label label1;
        private TextBox lblX;
        private TextBox lblY;
        private Label label2;
        private TextBox lblWidth;
        private Label label3;
        private TextBox lblHeight;
        private Label label4;
        private TextBox lblID;
        private Label label5;
        private Button btnRedraw;
        private Button btnSaveRegion;
        private Button btnClearRegion;
        private Button btnAddToFrame;
        private PictureBox pictureBox2;
        private Label label6;
        private TextBox lblImageId;
        private TextBox lblDY;
        private Label label7;
        private TextBox lblDX;
        private Label label8;
        private Label label9;
        private TextBox lblFrameID;
        private TextBox lblNumSub;
        private Label label10;
    }
}