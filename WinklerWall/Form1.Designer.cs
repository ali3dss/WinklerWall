namespace WinklerWall
{
    partial class Form1
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
            this.btnConstruirModelo = new System.Windows.Forms.Button();
            this.txtSalida = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtWallLength = new System.Windows.Forms.TextBox();
            this.txtExcavationDepth = new System.Windows.Forms.TextBox();
            this.txtThickness = new System.Windows.Forms.TextBox();
            this.txtElasticModulus = new System.Windows.Forms.TextBox();
            this.txtWidth = new System.Windows.Forms.TextBox();
            this.txtGamma = new System.Windows.Forms.TextBox();
            this.txtKh = new System.Windows.Forms.TextBox();
            this.txtElementLength = new System.Windows.Forms.TextBox();
            this.sdfdsf = new System.Windows.Forms.Label();
            this.txtPhi = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnConstruirModelo
            // 
            this.btnConstruirModelo.Location = new System.Drawing.Point(244, 685);
            this.btnConstruirModelo.Name = "btnConstruirModelo";
            this.btnConstruirModelo.Size = new System.Drawing.Size(210, 50);
            this.btnConstruirModelo.TabIndex = 0;
            this.btnConstruirModelo.Text = "Calcular";
            this.btnConstruirModelo.UseVisualStyleBackColor = true;
            this.btnConstruirModelo.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtSalida
            // 
            this.txtSalida.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSalida.Location = new System.Drawing.Point(59, 770);
            this.txtSalida.Multiline = true;
            this.txtSalida.Name = "txtSalida";
            this.txtSalida.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSalida.Size = new System.Drawing.Size(1300, 544);
            this.txtSalida.TabIndex = 1;
            this.txtSalida.WordWrap = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(55, 107);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(178, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Longitud de pantalla [m]";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(55, 166);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(224, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Profundidad de excavación [m]";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(55, 224);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Espesor [m]";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(55, 273);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(120, 20);
            this.label4.TabIndex = 5;
            this.label4.Text = "Módulo E [GPa]";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(55, 326);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(130, 20);
            this.label5.TabIndex = 6;
            this.label5.Text = "Ancho de cálculo";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(55, 599);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(108, 20);
            this.label6.TabIndex = 7;
            this.label6.Text = "Discretización";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(55, 553);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(222, 20);
            this.label7.TabIndex = 8;
            this.label7.Text = "Módulo de balasto KH [kN/m3]";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(55, 496);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(158, 20);
            this.label8.TabIndex = 9;
            this.label8.Text = "Peso unitario [kN/m3]";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(55, 385);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(132, 20);
            this.label10.TabIndex = 11;
            this.label10.Text = "Datos del terreno";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(55, 59);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(134, 20);
            this.label11.TabIndex = 12;
            this.label11.Text = "Datos de pantalla";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(55, 641);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(217, 20);
            this.label12.TabIndex = 13;
            this.label12.Text = "Longitud del elemento Dz (m)";
            // 
            // txtWallLength
            // 
            this.txtWallLength.Location = new System.Drawing.Point(344, 107);
            this.txtWallLength.Name = "txtWallLength";
            this.txtWallLength.Size = new System.Drawing.Size(110, 26);
            this.txtWallLength.TabIndex = 14;
            this.txtWallLength.Text = "10";
            // 
            // txtExcavationDepth
            // 
            this.txtExcavationDepth.Location = new System.Drawing.Point(344, 166);
            this.txtExcavationDepth.Name = "txtExcavationDepth";
            this.txtExcavationDepth.Size = new System.Drawing.Size(110, 26);
            this.txtExcavationDepth.TabIndex = 15;
            this.txtExcavationDepth.Text = "6";
            // 
            // txtThickness
            // 
            this.txtThickness.Location = new System.Drawing.Point(344, 224);
            this.txtThickness.Name = "txtThickness";
            this.txtThickness.Size = new System.Drawing.Size(110, 26);
            this.txtThickness.TabIndex = 16;
            this.txtThickness.Text = "0.60";
            // 
            // txtElasticModulus
            // 
            this.txtElasticModulus.Location = new System.Drawing.Point(344, 273);
            this.txtElasticModulus.Name = "txtElasticModulus";
            this.txtElasticModulus.Size = new System.Drawing.Size(110, 26);
            this.txtElasticModulus.TabIndex = 17;
            this.txtElasticModulus.Text = "30";
            // 
            // txtWidth
            // 
            this.txtWidth.Location = new System.Drawing.Point(344, 320);
            this.txtWidth.Name = "txtWidth";
            this.txtWidth.Size = new System.Drawing.Size(110, 26);
            this.txtWidth.TabIndex = 18;
            this.txtWidth.Text = "1";
            // 
            // txtGamma
            // 
            this.txtGamma.Location = new System.Drawing.Point(344, 490);
            this.txtGamma.Name = "txtGamma";
            this.txtGamma.Size = new System.Drawing.Size(110, 26);
            this.txtGamma.TabIndex = 20;
            this.txtGamma.Text = "18";
            // 
            // txtKh
            // 
            this.txtKh.Location = new System.Drawing.Point(344, 550);
            this.txtKh.Name = "txtKh";
            this.txtKh.Size = new System.Drawing.Size(110, 26);
            this.txtKh.TabIndex = 21;
            this.txtKh.Text = "20000";
            // 
            // txtElementLength
            // 
            this.txtElementLength.Location = new System.Drawing.Point(344, 635);
            this.txtElementLength.Name = "txtElementLength";
            this.txtElementLength.Size = new System.Drawing.Size(110, 26);
            this.txtElementLength.TabIndex = 22;
            this.txtElementLength.Text = "1";
            // 
            // sdfdsf
            // 
            this.sdfdsf.AutoSize = true;
            this.sdfdsf.Location = new System.Drawing.Point(55, 435);
            this.sdfdsf.Name = "sdfdsf";
            this.sdfdsf.Size = new System.Drawing.Size(152, 20);
            this.sdfdsf.TabIndex = 10;
            this.sdfdsf.Text = "Ángulo de friccion [°]";
            // 
            // txtPhi
            // 
            this.txtPhi.Location = new System.Drawing.Point(344, 432);
            this.txtPhi.Name = "txtPhi";
            this.txtPhi.Size = new System.Drawing.Size(110, 26);
            this.txtPhi.TabIndex = 19;
            this.txtPhi.Text = "30";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1497, 1336);
            this.Controls.Add(this.txtElementLength);
            this.Controls.Add(this.txtKh);
            this.Controls.Add(this.txtGamma);
            this.Controls.Add(this.txtPhi);
            this.Controls.Add(this.txtWidth);
            this.Controls.Add(this.txtElasticModulus);
            this.Controls.Add(this.txtThickness);
            this.Controls.Add(this.txtExcavationDepth);
            this.Controls.Add(this.txtWallLength);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.sdfdsf);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSalida);
            this.Controls.Add(this.btnConstruirModelo);
            this.Name = "Form1";
            this.Text = "Winkler - Pantalla en voladizo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConstruirModelo;
        private System.Windows.Forms.TextBox txtSalida;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtWallLength;
        private System.Windows.Forms.TextBox txtExcavationDepth;
        private System.Windows.Forms.TextBox txtThickness;
        private System.Windows.Forms.TextBox txtElasticModulus;
        private System.Windows.Forms.TextBox txtWidth;
        private System.Windows.Forms.TextBox txtGamma;
        private System.Windows.Forms.TextBox txtKh;
        private System.Windows.Forms.TextBox txtElementLength;
        private System.Windows.Forms.Label sdfdsf;
        private System.Windows.Forms.TextBox txtPhi;
    }
}

