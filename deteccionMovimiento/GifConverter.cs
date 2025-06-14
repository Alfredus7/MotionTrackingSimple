using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace deteccionMovimiento
{
    public static class GifConverter
    {
        /// <summary>
        /// Genera frames con trayectoria dibujada para crear un GIF animado
        /// </summary>
        public static List<Bitmap> GenerarFramesConTrayectoria(List<Bitmap> framesOriginales, List<Point> trayectoria, Action<Image, List<Point>, Point> dibujarTrayectoria)
        {
            if (framesOriginales == null || trayectoria == null)
                throw new ArgumentNullException("Los parámetros no pueden ser nulos");

            List<Bitmap> framesProcesados = new List<Bitmap>();

            for (int i = 0; i < framesOriginales.Count; i++)
            {
                Bitmap frame = new Bitmap(framesOriginales[i]);
                Point centroide = i < trayectoria.Count ? trayectoria[i] : Point.Empty;

                dibujarTrayectoria(frame,
                                 trayectoria.GetRange(0, Math.Min(i, trayectoria.Count)),
                                 centroide);

                framesProcesados.Add(frame);
            }

            return framesProcesados;
        }

        /// <summary>
        /// Guarda una lista de imágenes como GIF animado
        /// </summary>
        public static void GuardarComoGIF(List<Bitmap> frames, string filePath)
        {
            if (frames == null || frames.Count == 0)
                throw new ArgumentException("La lista de frames no puede estar vacía");

            ImageCodecInfo gifEncoder = GetEncoder(ImageFormat.Gif);
            using (var gif = frames[0])
            {
                // Guardar primer frame
                var encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
                gif.Save(filePath, gifEncoder, encoderParams);

                // Añadir frames adicionales
                encoderParams.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.FrameDimensionTime);
                for (int i = 1; i < frames.Count; i++)
                {
                    using (var frame = frames[i])
                    {
                        gif.SaveAdd(frame, encoderParams);
                    }
                }

                // Finalizar
                encoderParams.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.Flush);
                gif.SaveAdd(encoderParams);
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
            {
                if (codec.FormatID == format.Guid)
                    return codec;
            }
            return null;
        }
    }
}