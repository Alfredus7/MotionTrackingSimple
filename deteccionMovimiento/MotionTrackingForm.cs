using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
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
        private Timer _temporizadorAnimacion = new Timer { Interval = 100 }; // 10 FPS (100ms por frame)

        /// <summary>
        /// Inicializa el formulario y configura el doble buffer para mejor rendimiento gráfico
        /// </summary>
        public MotionTrackingForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true; // Para renderizado suave
            _temporizadorAnimacion.Tick += (sender, e) => ProcesarFrameActual();
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
            _temporizadorAnimacion.Stop();
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
            int sumaX = 0, sumaY = 0, contadorPixeles = 0;

            // Recorrer toda la imagen buscando píxeles del objeto (blancos)
            for (int y = 0; y < imagenBinaria.Height; y++)
            {
                for (int x = 0; x < imagenBinaria.Width; x++)
                {
                    // Verificar si el píxel es parte del objeto (componente rojo = 255)
                    if (imagenBinaria.GetPixel(x, y).R == 255)
                    {
                        sumaX += x;
                        sumaY += y;
                        contadorPixeles++;
                    }
                }
            }

            // Calcular promedio solo si se detectó un objeto
            return contadorPixeles > 0 ?
                new Point(sumaX / contadorPixeles, sumaY / contadorPixeles) :
                Point.Empty;
        }

        /// <summary>
        /// Dibuja la trayectoria del objeto y marca su posición actual
        /// </summary>
        /// <param name="imagen">Imagen sobre la que dibujar</param>
        /// <param name="trayectoria">Lista de puntos históricos</param>
        /// <param name="posicionActual">Posición actual del objeto</param>
        private void DibujarTrayectoriaObjeto(Image imagen, List<Point> trayectoria, Point posicionActual)
        {
            using (var g = Graphics.FromImage(imagen))
            {
                // Configuración de tamaños y colores
                float size = (float)numericSize.Value;
                var colors = new
                {
                    Trayectoria = Color.LimeGreen,   // Línea de trayectoria
                    Puntos = Color.DarkGreen,        // Puntos históricos
                    Actual = Color.Red,              // Marcador actual
                    Borde = Color.DarkRed            // Borde del marcador
                };

                // Dibujar trayectoria histórica (líneas + puntos)
                if (trayectoria.Count > 1)
                {
                    using (var pen = new Pen(colors.Trayectoria, size))
                        g.DrawLines(pen, trayectoria.ToArray());  // Une todos los puntos

                    // Dibuja círculos en cada posición histórica
                    using (var brush = new SolidBrush(colors.Puntos))
                        foreach (var p in trayectoria)
                            g.FillEllipse(brush, p.X - size, p.Y - size, size * 2, size * 2);
                }

                // Dibujar marcador de posición actual (círculo + cruz)
                if (!posicionActual.IsEmpty)
                {
                    float markerSize = size * 1.7f;


                    using (var brush = new SolidBrush(colors.Actual))
                    using (var pen = new Pen(Color.FromArgb(220, colors.Borde), size * 1.5f))
                    {
                        // Círculo principal
                        g.FillEllipse(brush, posicionActual.X - markerSize / 2, posicionActual.Y - markerSize / 2, markerSize, markerSize);
                        g.DrawEllipse(pen, posicionActual.X - markerSize / 2, posicionActual.Y - markerSize / 2, markerSize, markerSize);
                    }
                }
            }
        }

        /// <summary>
        /// Inicia o reinicia el procesamiento del GIF
        /// </summary>
        private void IniciarProcesamiento()
        {
            _temporizadorAnimacion.Stop();
            _indiceFrameActual = 0;
            _historialPosiciones.Clear();
            _temporizadorAnimacion.Start();
        }

        #endregion
    }
}