using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Reflection;

namespace Laugris.Sage
{
    /// <summary>
    /// Wrapper pour les CachedBitmap non managés de GDI+
    /// </summary>
    public sealed class CachedBitmap : IDisposable
    {
        private Bitmap _cachedBitmap;
        private Graphics _graphics;
        private IntPtr _pCachedBitmap;
        private FieldInfo _Bitmapfi;
        private FieldInfo _Graphicsfi;
        private IntPtr _pBitmapfi;
        private IntPtr _pGraphicsfi;
        private bool isLoaded = false;

        /// <summary>
        /// Constructeur.
        /// </summary>
        /// <param name="b">un objet Bitmap</param>
        /// <param name="g">un objet Graphics</param>
        public CachedBitmap(Bitmap b, Graphics g)
        {
            this.Init(b, g);
        }

        /// <summary>
        /// Destructeur
        /// </summary>
        ~CachedBitmap()
        {
            Dispose(false);
        }

        /// <summary>
        /// Utilisé pour initialiser un CachedBitmap à partir d'un Graphics et d'un Bitmap
        /// </summary>
        /// <param name="b">un objet Bitmap</param>
        /// <param name="g">un objet Graphics</param>
        public void Init(Bitmap b, Graphics g)
        {
            if (isLoaded) 
                this.Delete();

            this._cachedBitmap = b;
            this._graphics = g;
            this._Bitmapfi = typeof(Bitmap).GetField("nativeImage", BindingFlags.Instance | BindingFlags.NonPublic);
            this._pBitmapfi = (IntPtr)this._Bitmapfi.GetValue(this._cachedBitmap);
            this._Graphicsfi = typeof(Graphics).GetField("nativeGraphics", BindingFlags.Instance | BindingFlags.NonPublic);
            this._pGraphicsfi = (IntPtr)this._Graphicsfi.GetValue(this._graphics);
            NativeMethods.GdipCreateCachedBitmap(this._pBitmapfi, this._pGraphicsfi, ref this._pCachedBitmap);
            this.isLoaded = true;
        }

        /// <summary>
        /// Modifie seulement le Bitmap sans avoir à tout refaire (plus rapide que la méthode Init() ). Utile pour faire une animation par exemple.
        /// </summary>
        /// <param name="b">Un objet Bitmap</param>
        /// <returns></returns>
        /// <remarks>La mise à jour ne sera pas effectuée si la méthode</remarks>
        public bool InitBitmap(Bitmap b)
        {
            if (!isLoaded) return false;
            this.Delete();
            this._cachedBitmap = b;
            this._Bitmapfi = typeof(Bitmap).GetField("nativeImage", BindingFlags.Instance | BindingFlags.NonPublic);
            this._pBitmapfi = (IntPtr)this._Bitmapfi.GetValue(this._cachedBitmap);
            NativeMethods.GdipCreateCachedBitmap(this._pBitmapfi, this._pGraphicsfi, ref this._pCachedBitmap);
            this.isLoaded = true;
            return true;
        }

        /// <summary>
        /// Modifie seulement le Graphics où l'on dessinera le Bitmap (plus rapide que la méthode Init() ).
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public bool InitGraphics(Graphics g)
        {
            if (!isLoaded) return false;
            this.Delete();
            this._graphics = g;
            this._Graphicsfi = typeof(Graphics).GetField("nativeGraphics", BindingFlags.Instance | BindingFlags.NonPublic);
            this._pGraphicsfi = (IntPtr)this._Graphicsfi.GetValue(this._graphics);
            NativeMethods.GdipCreateCachedBitmap(this._pBitmapfi, this._pGraphicsfi, ref this._pCachedBitmap);
            this.isLoaded = true;
            return true;
        }

        /// <summary>
        /// Définit ou renvoie le Bitmap utilisé
        /// </summary>
        public Bitmap Bitmap
        {
            get
            {
                return _cachedBitmap;
            }
            set
            {
                this._cachedBitmap = value;
                this.InitBitmap(this._cachedBitmap);
            }
        }

        /// <summary>
        /// Définit ou renvoie le Graphics utilisé
        /// </summary>
        public Graphics Graphics
        {
            get
            {
                return _graphics;
            }
            set
            {
                this._graphics = value;
                this.InitGraphics(this._graphics);
            }
        }

        /// <summary>
        /// Vrai si l'initialisation du CachedBitmap s'est déroulée correctement
        /// </summary>
        public bool IsLoaded
        {
            get
            {
                return isLoaded;
            }
        }

        /// <summary>
        /// Dessine le Bitmap sur l'objet Graphics aux coordonnées (0 ; 0)
        /// </summary>
        /// <returns>Un entier</returns>
        public int Draw()
        {
            return this.Draw(0, 0);
        }

        /// <summary>
        /// Dessine le Bitmap sur l'objet Graphics aux coordonnées spécifiées
        /// </summary>
        /// <param name="x">Distance X du bord gauche du Graphics</param>
        /// <param name="y">Distance Y du bord haut du Graphics</param>
        /// <returns>Un entier</returns>
        public int Draw(int x, int y)
        {
            return NativeMethods.GdipDrawCachedBitmap(this._pGraphicsfi, this._pCachedBitmap, x, y);
        }

        /// <summary>
        /// Dessine le bitmap sur l'objet Graphics au coin haut-gauche spécifié par le Point
        /// </summary>
        /// <param name="p">Un objet Point qui désigne le coin haut-gauche où sera dessiné le Bitmap</param>
        /// <returns>Un entier</returns>
        public int Draw(Point p)
        {
            return this.Draw(p.X, p.Y);
        }

        /// <summary>
        /// Libère les ressources utilisées par le CachedBitmap non managé.
        /// </summary>
        public void Delete()
        {
            if (this.isLoaded)
            {
                NativeMethods.GdipDeleteCachedBitmap(this._pCachedBitmap);
                this.isLoaded = false;
            }
        }


        #region IDisposable Members

        internal void Dispose(bool disposing)
        {
            Delete();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}