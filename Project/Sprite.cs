// Beginning C# Game Programming
// (C)2004 Ron Penton
// Advanced Framework Sprite Class

using Drawing = System.Drawing;
using DirectX = Microsoft.DirectX;
using Direct3D = Microsoft.DirectX.Direct3D;


namespace AdvancedFramework
{

    public class Camera
    {
        DirectX.Vector2 position;
        DirectX.Vector2 screensize;
        DirectX.Vector2 finaloffset;

        public Camera( int screenx, int screeny, int x, int y )
        {
            initialize( screenx, screeny, x, y );
        }

        public Camera( int screenx, int screeny )
        {
            initialize( screenx, screeny, 0, 0 );
        }
        

        void initialize( int screenx, int screeny, int x, int y )
        {
            screensize.X = screenx;
            screensize.Y = screeny;
            position.X = x;
            position.Y = y;
            CalculateXOffset();
            CalculateYOffset();
        }


        void CalculateXOffset()
        {
            finaloffset.X = position.X - (screensize.X / 2);
        }

        void CalculateYOffset()
        {
            finaloffset.Y = position.Y - (screensize.Y / 2);
        }

        public float X
        {
            get { return position.X; }
            set { position.X = value; CalculateXOffset(); }
        }

        public float Y
        {
            get { return position.Y; }
            set { position.Y = value; CalculateYOffset(); }
        }

        public float ScreenX
        {
            get { return screensize.X; }
            set { screensize.X = value; CalculateXOffset(); }
        }

        public float ScreenY
        {
            get { return screensize.Y; }
            set { screensize.Y = value; CalculateYOffset(); }
        }

        public DirectX.Vector2 Offset
        {
            get { return finaloffset; }
        }

    }

    // --------------------------------------------------------------------
    // Sprite class
    // --------------------------------------------------------------------
    public class Sprite
    {

        // --------------------------------------------------------------------
        // variables
        // --------------------------------------------------------------------
        Direct3D.Texture texture;
        Drawing.Rectangle rect;

        DirectX.Vector2 scaling;
        DirectX.Vector2 offset;
        DirectX.Vector2 anchor;

        float angle;
        Drawing.Color color;


        // --------------------------------------------------------------------
        // Constructor
        // --------------------------------------------------------------------
        public Sprite()
        {
            // no texture
            texture = null;

            // scaling default
            scaling = new DirectX.Vector2( 1.0f, 1.0f );

            // no coloring
            color = Drawing.Color.White;
        }


        // --------------------------------------------------------------------
        // Texture property
        // --------------------------------------------------------------------
        public Direct3D.Texture Texture
        {
            get { return texture; }
            set 
            { 
                // set the texture
                texture = value; 

                // generate new rectangles
                rect = new Drawing.Rectangle( 0, 0, Width, Height );
            }
        }


        // --------------------------------------------------------------------
        // Width and Height of the sprite in pre-scaled pixels
        // --------------------------------------------------------------------
        public int Width
        {
            get { return texture.GetLevelDescription( 0 ).Width; }
        }
        public int Height
        {
            get { return texture.GetLevelDescription( 0 ).Height; }
        }


        // --------------------------------------------------------------------
        // Scale of sprite
        // --------------------------------------------------------------------
        public float XScale
        {
            get { return scaling.X; }
            set { scaling.X = value; }
        }

        public float YScale
        {
            get { return scaling.Y; }
            set { scaling.Y = value; }
        }


        public float Scale
        {   // helper to set both scales at once
            set { scaling.Y = value; scaling.X = value; }
        }


        
        // --------------------------------------------------------------------
        // Offset of sprite
        // --------------------------------------------------------------------
        public float X
        {
            get { return offset.X; }
            set { offset.X = value; }
        }
        public float Y
        {
            get { return offset.Y; }
            set { offset.Y = value; }
        }



        // --------------------------------------------------------------------
        // Rotation center of sprite, in the range of [0.0,1.0]
        // --------------------------------------------------------------------
        public float XAnchor
        {
            get { return (float)anchor.X; }
            set { anchor.X = value; }
        }
        public float YAnchor
        {
            get { return (float)anchor.Y; }
            set { anchor.Y = value; }
        }



        // --------------------------------------------------------------------
        // rotation angle of sprite
        // --------------------------------------------------------------------
        public float Angle
        {
            get { return angle; }
            set { angle = value; }
        }


        // --------------------------------------------------------------------
        // Color of sprite
        // --------------------------------------------------------------------
        public Drawing.Color Color
        {
            get { return color; }
            set { color = value; }
        }


        // --------------------------------------------------------------------
        // Tell a Direct3D.Sprite object how to render this sprite
        // --------------------------------------------------------------------
        public void Draw( Direct3D.Sprite renderer, Camera camera )
        {
            // calculate the actual width and height:
            float w = XScale * Width;
            float h = YScale * Height;

            // calculate the actual anchor point in sprite coordinates:
            DirectX.Vector2 scaledanchor = anchor;
            scaledanchor.X *= w;
            scaledanchor.Y *= h;

            DirectX.Vector2 newoffset = offset;
            newoffset.X -= scaledanchor.X;
            newoffset.Y -= scaledanchor.Y;


            renderer.Draw( 
                texture,
                rect,
                scaling,
                scaledanchor,
                angle,
                newoffset - camera.Offset,
                color );
        }


    }

}
