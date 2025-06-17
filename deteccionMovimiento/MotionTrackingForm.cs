using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using Accord.Imaging;
using Accord.Imaging.Filters;

namespace DeteccionMovimiento
{
    /// <summary>
    /// Formulario principal para el seguimiento de movimiento en imágenes
    /// Detecta y sigue objetos en movimiento en un GIF animado
    /// </summary>
    public partial class MotionTrackingForm : Form
    {
        // Lista de frames originales del GIF cargado
        private List<Bitmap> _framesOriginales = new List<Bitmap>();

        // Historial de posiciones del objeto detectado
        private List<Point> _historialPosiciones = new List<Point>();

        // Índice del frame actual que se está procesando
        private int _indiceFrameActual = 0;

        // Temporizador para controlar la velocidad de reproducción
        private Timer _temporizadorFrames = new Timer { Interval = 100 }; // 10 FPS (100ms por frame)

        /// <summary>
        /// Inicializa el formulario y configura el doble buffer para mejor rendimiento gráfico
        /// </summary>
        public MotionTrackingForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true; // Para renderizado suave
            _temporizadorFrames.Tick += (sender, e) => ProcesarFrameActual();
        }

        #region Eventos de Interfaz

        /// <summary>
        /// Maneja el evento de clic en el botón para cargar un archivo GIF
        /// </summary>
        private void BtnCargarGIF_Click(object sender, EventArgs e)
        {
            using (var dialogoArchivo = new OpenFileDialog
            {
                Filter = "GIF animado (*.gif)|*.gif",
                Title = "Seleccione un GIF animado para analizar"
            })
            {
                if (dialogoArchivo.ShowDialog() == DialogResult.OK)
                {
                    CargarFramesDesdeGIF(dialogoArchivo.FileName);
                }
            }

            IniciarProcesamiento();
            groupBoxOpciones.Enabled = true;
        }

        /// <summary>
        /// Maneja el evento de clic en el botón de procesamiento
        /// </summary>
        private void BtnProcesar_Click(object sender, EventArgs e)
        {
            IniciarProcesamiento();
        }

        /// <summary>
        /// Maneja cambios en la configuración de inversión de colores
        /// </summary>
        private void Pausar_Changed(object sender, EventArgs e)
        {
            _temporizadorFrames.Stop();
        }

        #endregion

        #region Procesamiento de Imágenes

        /// <summary>
        /// Carga un archivo GIF y extrae todos sus frames individuales
        /// </summary>
        /// <param name="rutaArchivo">Ruta completa del archivo GIF a cargar</param>
        private void CargarFramesDesdeGIF(string rutaArchivo)
        {
            // Limpiar frames anteriores
            _framesOriginales.ForEach(frame => frame?.Dispose());
            _framesOriginales.Clear();

            using (var imagenGIF = Accord.Imaging.Image.FromFile(rutaArchivo))
            {
                var dimensionFrames = new FrameDimension(imagenGIF.FrameDimensionsList[0]);
                int totalFrames = imagenGIF.GetFrameCount(dimensionFrames);

                // Extraer cada frame del GIF
                for (int i = 0; i < totalFrames; i++)
                {
                    imagenGIF.SelectActiveFrame(dimensionFrames, i);
                    var frame = new Bitmap(imagenGIF.Width, imagenGIF.Height);
                    using (var graficos = Graphics.FromImage(frame))
                        graficos.DrawImage(imagenGIF, 0, 0);
                    _framesOriginales.Add(frame);
                }
            }
        }

        /// <summary>
        /// Procesa el frame actual: detecta movimiento y actualiza la visualización
        /// </summary>
        private void ProcesarFrameActual()
        {
            if (_framesOriginales.Count == 0) return;

            // Obtener y mostrar el frame original
            var frameOriginal = _framesOriginales[_indiceFrameActual];
            pictureBoxOriginal.Image = frameOriginal;

            // Procesar el frame para detección de movimiento
            var frameProcesado = AplicarFiltros(frameOriginal);

            // Calcular y registrar la posición del objeto
            var centroObjeto = CalcularCentroObjeto(frameProcesado);
            if (!centroObjeto.IsEmpty) _historialPosiciones.Add(centroObjeto);

            // Dibujar la trayectoria del objeto
            DibujarTrayectoriaObjeto(frameProcesado, _historialPosiciones, centroObjeto);

            // Mostrar el frame procesado
            pictureBoxProcesada.Image = frameProcesado;

            // Avanzar al siguiente frame (circular)
            _indiceFrameActual = (_indiceFrameActual + 1) % _framesOriginales.Count;
        }

        /// <summary>
        /// Aplica una serie de filtros para resaltar el objeto en movimiento
        /// </summary>
        /// <param name="imagenOriginal">Imagen original a procesar</param>
        /// <returns>Imagen binaria procesada (blanco=objeto, negro=fondo)</returns>
        private Bitmap AplicarFiltros(Bitmap imagenOriginal)
        {
            // 1. Convertir a escala de grises (ponderación estándar)
            Bitmap imagenGrises = new Grayscale(0.2125, 0.7154, 0.0721).Apply(imagenOriginal);

            // 2. Opcional: Invertir colores (para que el fondo sea blanco)
            if (checkBoxInvertir.Checked)
            {
                new Invert().ApplyInPlace(imagenGrises);
            }
            // 3. Umbralización automática (método Otsu)
            new OtsuThreshold().ApplyInPlace(imagenGrises);
            // 4. Dilatación para unir áreas cercanas y reducir ruido
            new Dilation().ApplyInPlace(imagenGrises);
            // 5. Convertir a formato RGB estándar para visualización
            Bitmap resultado = new Bitmap(imagenGrises.Width, imagenGrises.Height, PixelFormat.Format24bppRgb);
            using (Graphics graficos = Graphics.FromImage(resultado))
                graficos.DrawImage(imagenGrises, 0, 0);

            imagenGrises.Dispose();
            return resultado;
        }

        /// <summary>
        /// Calcula el centro del objeto detectado en una imagen binaria
        /// </summary>
        /// <param name="imagenBinaria">Imagen procesada (blanco=objeto, negro=fondo)</param>
        /// <returns>
        /// Punto con las coordenadas del centro del objeto o Point.Empty si no se detecta
        /// </returns>
        private Point CalcularCentroObjeto(Bitmap imagenBinaria)
        {
            var image = UnmanagedImage.FromManagedImage(imagenBinaria);
            var whitePixels = Enumerable.Range(0, image.Height)
                .SelectMany(y => Enumerable.Range(0, image.Width)
                    .Where(x => image.GetPixel(x, y).R == 255)
                    .Select(x => new Point(x, y)))
                .ToList();

            return whitePixels.Count == 0 ? Point.Empty :
                new Point(
                    (int)Math.Round(whitePixels.Average(p => p.X)),
                    (int)Math.Round(whitePixels.Average(p => p.Y)));
        }

        /// <summary>
        /// Dibuja la trayectoria del objeto y marca su posición actual
        /// </summary>
        /// <param name="imagen">Imagen sobre la que dibujar</param>
        /// <param name="trayectoria">Lista de puntos históricos</param>
        /// <param name="posicionActual">Posición actual del objeto</param>
        private void DibujarTrayectoriaObjeto(Bitmap imagen, List<Point> trayectoria, Point posicionActual)
        {
            using (var graficos = Graphics.FromImage(imagen))
            {
                float tamaño = (float)numericSize.Value;
                var colorTrayectoria = Color.LimeGreen;
                var colorPuntos = Color.DarkGreen;
                var colorActual = Color.Red;

                // Dibujar trayectoria si hay suficiente historia
                if (trayectoria.Count > 1)
                {
                    // Líneas de trayectoria
                    using (var lapiz = new Pen(colorTrayectoria, tamaño))
                        graficos.DrawLines(lapiz, trayectoria.ToArray());

                    // Puntos históricos
                    using (var brocha = new SolidBrush(colorPuntos))
                        foreach (var punto in trayectoria)
                            graficos.FillEllipse(brocha, punto.X - tamaño, punto.Y - tamaño, tamaño * 2, tamaño * 2);
                }

                // Dibujar posición actual
                if (!posicionActual.IsEmpty)
                {
                    using (var brocha = new SolidBrush(colorActual))
                        graficos.FillEllipse(brocha, posicionActual.X - tamaño, posicionActual.Y - tamaño, tamaño * 2, tamaño * 2);
                }
            }
        }

        /// <summary>
        /// Inicia o reinicia el procesamiento del GIF
        /// </summary>
        private void IniciarProcesamiento()
        {
            _temporizadorFrames.Stop();
            _indiceFrameActual = 0;
            _historialPosiciones.Clear();
            _temporizadorFrames.Start();
        }

        #endregion
    }
}