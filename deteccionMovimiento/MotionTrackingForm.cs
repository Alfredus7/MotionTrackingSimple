using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Accord.Imaging.Filters;

namespace deteccionMovimiento
{
    /// <summary>
    /// Formulario principal para el seguimiento de movimiento simple
    /// Detecta y sigue un único objeto en movimiento en un GIF animado
    /// </summary>
    public partial class MotionTrackingForm : Form
    {
        // Almacena los frames originales del GIF cargado
        private List<Bitmap> originalFrames = new List<Bitmap>();

        // Guarda la trayectoria del objeto detectado (coordenadas X,Y)
        private List<Point> trayectoria = new List<Point>();

        // Índice del frame actual que se está procesando
        private int gifFrameIndex = 0;

        // Temporizador para controlar la animación
        private Timer timer = new Timer { Interval = 100 }; // 10 FPS (100ms por frame)

        /// <summary>
        /// Constructor del formulario
        /// </summary>
        public MotionTrackingForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true; // Para renderizado suave
            timer.Tick += (s, e) => ProcesarFrame();
        }

        /// <summary>
        /// Evento del botón para cargar un GIF animado
        /// </summary>
        private void CargarGIF_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog
            {
                Filter = "GIF animado (*.gif)|*.gif",
                Title = "Seleccione un GIF animado para analizar"
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    CargarFramesGIF(ofd.FileName);
                    
                }
            }
            Empezar();
            groupBox1.Enabled = true;
        }

        /// <summary>
        /// Carga un archivo GIF y extrae todos sus frames
        /// </summary>
        /// <param name="path">Ruta del archivo GIF a cargar</param>
        private void CargarFramesGIF(string path)
        {
            // Limpiar frames anteriores
            originalFrames.ForEach(frame => frame?.Dispose());
            originalFrames.Clear();

            using (var gif = Accord.Imaging.Image.FromFile(path))
            {
                var dimension = new FrameDimension(gif.FrameDimensionsList[0]);
                int frameCount = gif.GetFrameCount(dimension);

                // Extraer cada frame del GIF
                for (int i = 0; i < frameCount; i++)
                {
                    gif.SelectActiveFrame(dimension, i);
                    var frame = new Bitmap(gif.Width, gif.Height);
                    using (var g = Graphics.FromImage(frame))
                        g.DrawImage(gif, 0, 0);
                    originalFrames.Add(frame);
                }
            }
            
        }

        /// <summary>
        /// Procesa el frame actual: detecta movimiento y actualiza la visualización
        /// </summary>
        private void ProcesarFrame()
        {
            if (originalFrames.Count == 0) return;

            var original = originalFrames[gifFrameIndex];
            pictureBoxOriginal.Image = original;

            var processed = AplicarFiltros(original);
            var centroide = CalcularCentroide(processed);
            if (!centroide.IsEmpty) trayectoria.Add(centroide);

            // Usamos el nuevo método para dibujar la trayectoria
            DibujarTrayectoria(processed, trayectoria, centroide);

            pictureBoxProcesada.Image = processed;

            gifFrameIndex = (gifFrameIndex + 1) % originalFrames.Count;
        }

        /// <summary>
        /// Aplica filtros a la imagen para resaltar el objeto en movimiento
        /// </summary>
        /// <param name="original">Imagen original a procesar</param>
        /// <returns>Imagen binaria procesada</returns>
        private Bitmap AplicarFiltros(Bitmap original)
        {
            // Convertir a escala de grises (ponderación estándar)
            Bitmap gris = new Grayscale(0.2125, 0.7154, 0.0721).Apply(original);
            if (checkBoxInvertir.Checked)
            {
                //Invertir colores(para que el objeto sea blanco)
                new Invert().ApplyInPlace(gris);
            }
            // Umbralización automática (Otsu)
            new OtsuThreshold().ApplyInPlace(gris);
            // Dilatación para unir áreas cercanas
            new Dilation().ApplyInPlace(gris);
            // Convertir a formato RGB estándar
            Bitmap result = new Bitmap(gris.Width, gris.Height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(result))
                g.DrawImage(gris, 0, 0);

            gris.Dispose();
            return result;
        }

        /// <summary>
        /// Calcula el centroide (centro de masa) de los píxeles blancos
        /// </summary>
        /// <param name="bmp">Imagen binaria (blanco=objeto)</param>
        /// <returns>Punto con las coordenadas del centroide o Point.Empty si no se detecta objeto</returns>
        private Point CalcularCentroide(Bitmap bmp)
        {
            unsafe
            {
                int sumX = 0, sumY = 0, count = 0;

                // Recorrer toda la imagen buscando píxeles blancos (R=255)
                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        if (bmp.GetPixel(x, y).R == 255) // Píxel blanco detectado
                        {
                            sumX += x; // Acumular coordenada X
                            sumY += y; // Acumular coordenada Y
                            count++;   // Contar píxeles del objeto
                        }
                    }
                }

                // Devolver punto promedio (centroide) o vacío si no hay detección
                return count > 0 ? new Point(sumX / count, sumY / count) : Point.Empty;
            }
        }  
        private void btnProsesar_Click(object sender, EventArgs e)
        {
            Empezar();
        }

        private void DibujarTrayectoria(Image img, List<Point> path, Point center)
        {
            using (var g = Graphics.FromImage(img))
            {
                // Configuración básica
                float s = (float)numericSize.Value; // Tamaño base
                var colors = new
                {
                    Path = Color.LimeGreen,    // Color de la trayectoria
                    Point = Color.DarkGreen,    // Color de puntos
                    Current = Color.Red,        // Color del centro actual
                    Border = Color.DarkRed      // Color del borde
                };

                // Dibujar trayectoria (líneas + puntos)
                if (path.Count > 1)
                {
                    // Líneas de trayectoria
                    using (var pen = new Pen(colors.Path, s))
                        for (int i = 1; i < path.Count; i++)
                            g.DrawLine(pen, path[i - 1], path[i]);

                    // Puntos de trayectoria
                    using (var brush = new SolidBrush(colors.Point))
                        foreach (var p in path)
                            g.FillEllipse(brush, p.X - s, p.Y - s, s * 2, s * 2); // radio = size
                }

                // Marcador de posición actual
                if (!center.IsEmpty)
                {
                    float r = (float)(s * 1.5); // Radio del marcador
                    float cross = r * 0.7f; // Tamaño cruz

                    using (var brush = new SolidBrush(colors.Current))
                    using (var pen = new Pen(Color.FromArgb(220, colors.Border), s * 1.5f))
                    {
                        // Círculo principal
                        g.FillEllipse(brush, center.X - r / 2, center.Y - r / 2, r, r);
                        g.DrawEllipse(pen, center.X - r / 2, center.Y - r / 2, r, r);

                        // Cruz indicadora
                        g.DrawLine(pen, center.X - cross / 2, center.Y, center.X + cross / 2, center.Y);
                        g.DrawLine(pen, center.X, center.Y - cross / 2, center.X, center.Y + cross / 2);
                    }
                }
            }
        }












        private void GuardarGIF_Click(object sender, EventArgs e)
        {
            if (originalFrames.Count == 0 || trayectoria.Count == 0)
            {
                MessageBox.Show("No hay datos para guardar. Primero cargue un GIF y procese algunos frames.");
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "GIF animado (*.gif)|*.gif";
                sfd.Title = "Guardar GIF con trayectoria";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Generar frames usando la clase GifConverter
                        var framesConTrayectoria = GifConverter.GenerarFramesConTrayectoria(
                            originalFrames,
                            trayectoria,
                            DibujarTrayectoria);

                        // Guardar el GIF
                        GifConverter.GuardarComoGIF(framesConTrayectoria, sfd.FileName);

                        // Liberar recursos
                        framesConTrayectoria.ForEach(f => f.Dispose());

                        MessageBox.Show("GIF guardado exitosamente!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al guardar el GIF: {ex.Message}");
                    }
                }
            }
        }

        

        public void Empezar()
        {
            timer.Stop();
            gifFrameIndex = 0;
            trayectoria.Clear();
            timer.Start();
        }
        private void checkBoxInvertir_CheckedChanged(object sender, EventArgs e)
        {
            timer.Stop();
        }
        private void numericUmbral_ValueChanged(object sender, EventArgs e)
        {
            timer.Stop();
        }

        private void numericSize_ValueChanged(object sender, EventArgs e)
        {
            timer.Stop();
        }
    }
}