namespace DeteccionMovimiento
{
    partial class MotionTrackingForm
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MotionTrackingForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.cargarGIFMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBoxOriginal = new System.Windows.Forms.PictureBox();
            this.pictureBoxProcesada = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxOpciones = new System.Windows.Forms.GroupBox();
            this.numericSize = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBoxInvertir = new System.Windows.Forms.CheckBox();
            this.btnProcesar = new System.Windows.Forms.Button();
            this.lbFrames = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOriginal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProcesada)).BeginInit();
            this.groupBoxOpciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericSize)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cargarGIFMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "opciones";
            // 
            // cargarGIFMenuItem
            // 
            this.cargarGIFMenuItem.Name = "cargarGIFMenuItem";
            this.cargarGIFMenuItem.Size = new System.Drawing.Size(72, 20);
            this.cargarGIFMenuItem.Text = "cargar GIF";
            this.cargarGIFMenuItem.Click += new System.EventHandler(this.BtnCargarGIF_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Outset;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureBoxOriginal, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.pictureBoxProcesada, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 71.3615F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 377);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(404, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(391, 45);
            this.label2.TabIndex = 4;
            this.label2.Text = "Imagen Prosesada";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBoxOriginal
            // 
            this.pictureBoxOriginal.BackColor = System.Drawing.Color.White;
            this.pictureBoxOriginal.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxOriginal.BackgroundImage")));
            this.pictureBoxOriginal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxOriginal.Location = new System.Drawing.Point(5, 52);
            this.pictureBoxOriginal.Name = "pictureBoxOriginal";
            this.pictureBoxOriginal.Size = new System.Drawing.Size(391, 320);
            this.pictureBoxOriginal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxOriginal.TabIndex = 2;
            this.pictureBoxOriginal.TabStop = false;
            // 
            // pictureBoxProcesada
            // 
            this.pictureBoxProcesada.BackColor = System.Drawing.Color.White;
            this.pictureBoxProcesada.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxProcesada.BackgroundImage")));
            this.pictureBoxProcesada.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxProcesada.Location = new System.Drawing.Point(404, 52);
            this.pictureBoxProcesada.Name = "pictureBoxProcesada";
            this.pictureBoxProcesada.Size = new System.Drawing.Size(391, 320);
            this.pictureBoxProcesada.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxProcesada.TabIndex = 1;
            this.pictureBoxProcesada.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(5, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(391, 45);
            this.label1.TabIndex = 3;
            this.label1.Text = "Imagen Original";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBoxOpciones
            // 
            this.groupBoxOpciones.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.groupBoxOpciones.Controls.Add(this.lbFrames);
            this.groupBoxOpciones.Controls.Add(this.numericSize);
            this.groupBoxOpciones.Controls.Add(this.label4);
            this.groupBoxOpciones.Controls.Add(this.checkBoxInvertir);
            this.groupBoxOpciones.Controls.Add(this.btnProcesar);
            this.groupBoxOpciones.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBoxOpciones.Location = new System.Drawing.Point(0, 407);
            this.groupBoxOpciones.Name = "groupBoxOpciones";
            this.groupBoxOpciones.Size = new System.Drawing.Size(800, 43);
            this.groupBoxOpciones.TabIndex = 2;
            this.groupBoxOpciones.TabStop = false;
            this.groupBoxOpciones.Text = "opciones";
            // 
            // numericSize
            // 
            this.numericSize.Location = new System.Drawing.Point(301, 14);
            this.numericSize.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericSize.Name = "numericSize";
            this.numericSize.ReadOnly = true;
            this.numericSize.Size = new System.Drawing.Size(120, 20);
            this.numericSize.TabIndex = 10;
            this.numericSize.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numericSize.ValueChanged += new System.EventHandler(this.Pausar_Changed);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(201, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(94, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "tamaño trayectoria";
            // 
            // checkBoxInvertir
            // 
            this.checkBoxInvertir.AutoSize = true;
            this.checkBoxInvertir.Location = new System.Drawing.Point(137, 15);
            this.checkBoxInvertir.Name = "checkBoxInvertir";
            this.checkBoxInvertir.Size = new System.Drawing.Size(58, 17);
            this.checkBoxInvertir.TabIndex = 6;
            this.checkBoxInvertir.Text = "Invertir";
            this.checkBoxInvertir.UseVisualStyleBackColor = true;
            this.checkBoxInvertir.CheckedChanged += new System.EventHandler(this.Pausar_Changed);
            // 
            // btnProcesar
            // 
            this.btnProcesar.Location = new System.Drawing.Point(546, 12);
            this.btnProcesar.Name = "btnProcesar";
            this.btnProcesar.Size = new System.Drawing.Size(101, 23);
            this.btnProcesar.TabIndex = 5;
            this.btnProcesar.Text = "Procesar";
            this.btnProcesar.UseVisualStyleBackColor = true;
            this.btnProcesar.Click += new System.EventHandler(this.BtnProcesar_Click);
            // 
            // lbFrames
            // 
            this.lbFrames.AutoSize = true;
            this.lbFrames.Location = new System.Drawing.Point(427, 17);
            this.lbFrames.Name = "lbFrames";
            this.lbFrames.Size = new System.Drawing.Size(65, 13);
            this.lbFrames.TabIndex = 11;
            this.lbFrames.Text = "Frame: 0 / 0";
            // 
            // MotionTrackingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBoxOpciones);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MotionTrackingForm";
            this.Text = "Motion Tracking";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOriginal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProcesada)).EndInit();
            this.groupBoxOpciones.ResumeLayout(false);
            this.groupBoxOpciones.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem cargarGIFMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pictureBoxProcesada;
        private System.Windows.Forms.PictureBox pictureBoxOriginal;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxOpciones;
        private System.Windows.Forms.Button btnProcesar;
        private System.Windows.Forms.CheckBox checkBoxInvertir;
        private System.Windows.Forms.NumericUpDown numericSize;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbFrames;
    }
}

