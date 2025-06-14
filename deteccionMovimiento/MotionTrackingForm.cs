using System;
using System.Collections.Generic;
using System.Drawing;
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
                    CargarGIF(ofd.FileName);
                    gifFrameIndex = 0;
                    trayectoria.Clear();
                    timer.Start();
                }
            }
            groupBox1.Enabled = true;
        }

        /// <summary>
        /// Carga un archivo GIF y extrae todos sus frames
        /// </summary>
        /// <param name="path">Ruta del archivo GIF a cargar</param>
        private void CargarGIF(string path)
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

            // Obtener frame original actual
            var original = originalFrames[gifFrameIndex];
            pictureBoxOriginal.Image = original;

            // Procesar imagen para detección de movimiento
            var processed = AplicarFiltros(original);

            // Calcular centroide del objeto en movimiento
            var centroide = CalcularCentroide(processed);

            // Registrar posición en la trayectoria (si se detectó objeto)
            if (!centroide.IsEmpty) trayectoria.Add(centroide);

            // Dibujar trayectoria y marcadores
            using (var g = Graphics.FromImage(processed))
            {
                // Dibujar trayectoria histórica
                if (trayectoria.Count > 1)
                {
                    for (int i = 1; i < trayectoria.Count; i++)
                    {
                        // Línea conectando puntos anteriores
                        g.DrawLine(Pens.GreenYellow, trayectoria[i - 1], trayectoria[i]);
                        // Puntos pequeños verdes para posiciones anteriores
                        g.FillEllipse(Brushes.DarkGreen, trayectoria[i].X - 3, trayectoria[i].Y - 3, 6, 6);
                    }
                }

                // Dibujar posición actual (si hay detección)
                if (!centroide.IsEmpty)
                {
                    // Punto rojo grande para posición actual
                    g.FillEllipse(Brushes.Red, centroide.X - 5, centroide.Y - 5, 10, 10);
                    // Círculo exterior para mejor visibilidad
                    g.DrawEllipse(Pens.DarkRed, centroide.X - 7, centroide.Y - 7, 14, 14);
                }
            }

            // Actualizar PictureBox con el resultado procesado
            pictureBoxProcesada.Image?.Dispose();
            pictureBoxProcesada.Image = processed;

            // Avanzar al siguiente frame (cíclico)
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

            // Invertir colores (para que el objeto sea blanco)
            new Invert().ApplyInPlace(gris);

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
    }
}