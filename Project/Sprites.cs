// Beginning C# Game Programming
// (C)2004 Ron Penton
// Chapter 7
// Demo 5 - Sprites

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;


using Microsoft.DirectX;
using Direct3D = Microsoft.DirectX.Direct3D;
using DirectSound = Microsoft.DirectX.DirectSound;
using DirectInput = Microsoft.DirectX.DirectInput;


namespace AdvancedFramework
{
	// ------------------------------------------------------------------------
	// The main Game class.
	// ------------------------------------------------------------------------
	public class Game : Form
	{
		// --------------------------------------------------------------------
		// static variables
		// --------------------------------------------------------------------
		static string gametitle  = "Shotgun Fun Fun";
		static int screenwidth   = 1000;
		static int screenheight  = 500;
		static bool windowed     = true;
		static bool graphicslost = false;
		static Timer gametimer   = null;
		static bool paused       = false;
		static int soldatbo 	 = 0;
		static bool escales      = false;
		static int armabona      = 0;
		static bool pulsaRight   = false;
		static bool pulsaW       = false;
		static bool pulsaLeft    = false;
		static bool pulsaUp      = false;
		static bool pulsaDown    = false;
		static bool pulsaS       = false;
		static bool pulsaEspai   = false;
		//static float movingxbala   = 0;
		//static float movingybala   = 0;
		static int tirActual       = 0;
		static bool potDisparar    = false;
		static int numVectors      = 0;
		static bool gravetat       = true;
		static bool evitarPujar    = false;
		static int tempsAparicio   = 0;
		static int demonActual     = 0;
		static int reciclar        = 0;
		//static bool paret          = false;
		static bool potSaltar        = true;
		static bool antiVol          = false;
		
		
		//static int soldatbor     = 0;
		
		// --------------------------------------------------------------------
		// Devices
		// --------------------------------------------------------------------
		Direct3D.Device graphics        = null;
		DirectSound.Device sound        = null;
		DirectInput.Device keyboard     = null;
		DirectInput.Device mouse        = null;
		DirectInput.Device gameinput    = null;


		// --------------------------------------------------------------------
		// Geometry
		// --------------------------------------------------------------------
		//Direct3D.Texture texture = null;
		Direct3D.Texture fons     = null;
		Direct3D.Texture soldat1  = null;
		Direct3D.Texture soldat2  = null;
		Direct3D.Texture soldat3  = null;
		Direct3D.Texture soldat4  = null;
		Direct3D.Texture soldat5  = null;
		Direct3D.Texture soldat6  = null;
		Direct3D.Texture soldat1r = null;
		Direct3D.Texture soldat2r = null;
		Direct3D.Texture soldat3r = null;
		Direct3D.Texture soldat4r = null;
		Direct3D.Texture soldat5r = null;
		Direct3D.Texture soldat6r = null;
		Direct3D.Texture demon    = null;
		Direct3D.Texture demonr   = null;
		Direct3D.Texture armaTexture     = null;
		Direct3D.Texture armaTexturer    = null;
		Direct3D.Texture bala     = null;
		Direct3D.Texture balar    = null;
		
		Direct3D.Sprite spriterenderer = null;
		Sprite[] sprites = null;
		Sprite[] arma    = null;
		Sprite[] bales   = null;
		Sprite[] demons  = null;
		float [] movingxbales = null;
		float [] movingybales = null;
		int numsprites = 13;
		Camera camera = null;
		float movingx = 0.0f;
		float movingy = 0.0f;
		float [] movingxdemons = null;
		float [] movingydemons = null;

		// --------------------------------------------------------------------
		// Game constructor
		// --------------------------------------------------------------------
		public Game()
		{
			ClientSize = new System.Drawing.Size( screenwidth, screenheight );
			Text = gametitle;
			gametimer = new Timer();
		}
		

		// --------------------------------------------------------------------
		// Initialize the Direct3D Graphics subsystem
		// --------------------------------------------------------------------
		public void InitializeGraphics()
		{
			// set up the parameters
			Direct3D.PresentParameters p = new Direct3D.PresentParameters();
			p.SwapEffect = Direct3D.SwapEffect.Discard;

			if( windowed == true )
			{
				p.Windowed = true;
			}
			else
			{
				// get the current display mode:
				Direct3D.Format current = Direct3D.Manager.Adapters[0].CurrentDisplayMode.Format;

				// set up a fullscreen display device
				p.Windowed   = false;               // fullscreen
				p.BackBufferCount = 1;              // one back buffer
				p.BackBufferFormat = current;       // use current format
				p.BackBufferWidth = screenwidth;
				p.BackBufferHeight = screenheight;
			}

			// create a new device:
			graphics = new Direct3D.Device( 0, Direct3D.DeviceType.Hardware, this, Direct3D.CreateFlags.SoftwareVertexProcessing, p );


			// Setup the event handlers for the device
			graphics.DeviceLost     += new EventHandler( this.InvalidateDeviceObjects );
			graphics.DeviceReset    += new EventHandler( this.RestoreDeviceObjects );
			graphics.Disposing      += new EventHandler( this.DeleteDeviceObjects );
			graphics.DeviceResizing += new CancelEventHandler( this.EnvironmentResizing );


			// set up various drawing options
			graphics.RenderState.CullMode = Direct3D.Cull.None;
			graphics.RenderState.AlphaBlendEnable = true;
			graphics.RenderState.AlphaBlendOperation = Direct3D.BlendOperation.Add;
			graphics.RenderState.DestinationBlend = Direct3D.Blend.InvSourceAlpha;
			graphics.RenderState.SourceBlend = Direct3D.Blend.SourceAlpha;
		}


		// --------------------------------------------------------------------
		// Initialize the DirectSound subsystem
		// --------------------------------------------------------------------
		public void InitializeSound()
		{
			// set up a device
			sound = new DirectSound.Device();
			sound.SetCooperativeLevel( this, DirectSound.CooperativeLevel.Normal );
		}

		// --------------------------------------------------------------------
		// Initialize the DirectInput subsystem
		// --------------------------------------------------------------------
		public void InitializeInput()
		{
			// set up the keyboard
			keyboard = new DirectInput.Device( DirectInput.SystemGuid.Keyboard );
			keyboard.SetCooperativeLevel(
				this,
				DirectInput.CooperativeLevelFlags.Background |
				DirectInput.CooperativeLevelFlags.NonExclusive );
			keyboard.Acquire();

			// set up the mouse
			mouse = new DirectInput.Device( DirectInput.SystemGuid.Mouse );
			mouse.SetCooperativeLevel(
				this,
				DirectInput.CooperativeLevelFlags.Background |
				DirectInput.CooperativeLevelFlags.NonExclusive );
			mouse.Acquire();
		}


		// --------------------------------------------------------------------
		// Initialize the Geometry
		// --------------------------------------------------------------------
		public void InitializeResources()
		{
			// load the texture
			
			soldat1 = Direct3D.TextureLoader.FromFile(
				graphics, "soldier1.bmp", 50, 61, 0, 0, Direct3D.Format.Unknown,
				Direct3D.Pool.Managed, Direct3D.Filter.Linear,
				Direct3D.Filter.Linear, Color.FromArgb( 0, 0, 255 ).ToArgb() );
			soldat2 = Direct3D.TextureLoader.FromFile(
				graphics, "soldier2.bmp", 50, 61, 0, 0, Direct3D.Format.Unknown,
				Direct3D.Pool.Managed, Direct3D.Filter.Linear,
				Direct3D.Filter.Linear, Color.FromArgb( 0, 0, 255 ).ToArgb() );
			soldat3 = Direct3D.TextureLoader.FromFile(
				graphics, "soldier3.bmp", 50, 61, 0, 0, Direct3D.Format.Unknown,
				Direct3D.Pool.Managed, Direct3D.Filter.Linear,
				Direct3D.Filter.Linear, Color.FromArgb( 0, 0, 255 ).ToArgb() );
			soldat4 = Direct3D.TextureLoader.FromFile(
				graphics, "soldier4.bmp", 50, 61, 0, 0, Direct3D.Format.Unknown,
				Direct3D.Pool.Managed, Direct3D.Filter.Linear,
				Direct3D.Filter.Linear, Color.FromArgb( 0, 0, 255 ).ToArgb() );
			soldat5 = Direct3D.TextureLoader.FromFile(
				graphics, "soldier5.bmp", 50, 61, 0, 0, Direct3D.Format.Unknown,
				Direct3D.Pool.Managed, Direct3D.Filter.Linear,
				Direct3D.Filter.Linear, Color.FromArgb( 0, 0, 255 ).ToArgb() );
			soldat6 = Direct3D.TextureLoader.FromFile(
				graphics, "soldier6.bmp", 50, 61, 0, 0, Direct3D.Format.Unknown,
				Direct3D.Pool.Managed, Direct3D.Filter.Linear,
				Direct3D.Filter.Linear, Color.FromArgb( 0, 0, 255 ).ToArgb() );
			soldat1r = Direct3D.TextureLoader.FromFile(
				graphics, "soldier1r.bmp", 50, 61, 0, 0, Direct3D.Format.Unknown,
				Direct3D.Pool.Managed, Direct3D.Filter.Linear,
				Direct3D.Filter.Linear, Color.FromArgb( 0, 0, 255 ).ToArgb() );
			soldat2r = Direct3D.TextureLoader.FromFile(
				graphics, "soldier2r.bmp", 50, 61, 0, 0, Direct3D.Format.Unknown,
				Direct3D.Pool.Managed, Direct3D.Filter.Linear,
				Direct3D.Filter.Linear, Color.FromArgb( 0, 0, 255 ).ToArgb() );
			soldat3r = Direct3D.TextureLoader.FromFile(
				graphics, "soldier3r.bmp", 50, 61, 0, 0, Direct3D.Format.Unknown,
				Direct3D.Pool.Managed, Direct3D.Filter.Linear,
				Direct3D.Filter.Linear, Color.FromArgb( 0, 0, 255 ).ToArgb() );
			soldat4r = Direct3D.TextureLoader.FromFile(
				graphics, "soldier4r.bmp", 50, 61, 0, 0, Direct3D.Format.Unknown,
				Direct3D.Pool.Managed, Direct3D.Filter.Linear,
				Direct3D.Filter.Linear, Color.FromArgb( 0, 0, 255 ).ToArgb() );
			soldat5r = Direct3D.TextureLoader.FromFile(
				graphics, "soldier5r.bmp", 50, 61, 0, 0, Direct3D.Format.Unknown,
				Direct3D.Pool.Managed, Direct3D.Filter.Linear,
				Direct3D.Filter.Linear, Color.FromArgb( 0, 0, 255 ).ToArgb() );
			soldat6r = Direct3D.TextureLoader.FromFile(
				graphics, "soldier6r.bmp", 50, 61, 0, 0, Direct3D.Format.Unknown,
				Direct3D.Pool.Managed, Direct3D.Filter.Linear,
				Direct3D.Filter.Linear, Color.FromArgb( 0, 0, 255 ).ToArgb() );
			fons = Direct3D.TextureLoader.FromFile(
				graphics, "fons.bmp", 800, 500, 0, 0, Direct3D.Format.Unknown,
				Direct3D.Pool.Managed, Direct3D.Filter.Linear,
				Direct3D.Filter.Linear, Color.FromArgb( 0, 0, 0 ).ToArgb() );
			armaTexture = Direct3D.TextureLoader.FromFile(
				graphics, "arma.bmp", 61,25,0,0, Direct3D.Format.Unknown,
				Direct3D.Pool.Managed, Direct3D.Filter.Linear,
				Direct3D.Filter.Linear, Color.FromArgb(0,0, 255).ToArgb());
			armaTexturer = Direct3D.TextureLoader.FromFile(
				graphics, "armar.bmp", 61,25,0,0, Direct3D.Format.Unknown,
				Direct3D.Pool.Managed, Direct3D.Filter.Linear,
				Direct3D.Filter.Linear, Color.FromArgb(0,0, 255).ToArgb());
			bala = Direct3D.TextureLoader.FromFile(
				graphics, "bala.bmp", 61,25,0,0, Direct3D.Format.Unknown,
				Direct3D.Pool.Managed, Direct3D.Filter.Linear,
				Direct3D.Filter.Linear, Color.FromArgb(0,0, 255).ToArgb());
			balar = Direct3D.TextureLoader.FromFile(
				graphics, "balar.bmp", 61,25,0,0, Direct3D.Format.Unknown,
				Direct3D.Pool.Managed, Direct3D.Filter.Linear,
				Direct3D.Filter.Linear, Color.FromArgb(0,0, 255).ToArgb());
			demon = Direct3D.TextureLoader.FromFile(
				graphics, "demon.bmp", 50,61,0,0, Direct3D.Format.Unknown,
				Direct3D.Pool.Managed, Direct3D.Filter.Linear,
				Direct3D.Filter.Linear, Color.FromArgb(0,0, 255).ToArgb());
			demonr = Direct3D.TextureLoader.FromFile(
				graphics, "demonr.bmp", 50,61,0,0, Direct3D.Format.Unknown,
				Direct3D.Pool.Managed, Direct3D.Filter.Linear,
				Direct3D.Filter.Linear, Color.FromArgb(0,0, 255).ToArgb());
			
			// create a renderer
			spriterenderer = new Direct3D.Sprite( graphics );

			// create the sprites
			sprites = new Sprite[numsprites];
			arma    = new Sprite[2];
			bales   = new Sprite[10];
			demons  = new Sprite[25];//100000
			movingxbales = new float[10];
			movingybales = new float[10];
			movingxdemons = new float[25];
			movingydemons = new float[25];

			// auto-generate position/size/rotation information
			
			sprites[0] = new Sprite();
			sprites[0].Texture = soldat1;
			sprites[0].Scale = 1.0f;
			sprites[0].X = -480;
			sprites[0].Y = 45;
			sprites[0].Angle = 0;
			sprites[0].XAnchor = 0.5f;
			sprites[0].YAnchor = 0.5f;
			
			
			sprites[1] = new Sprite();
			sprites[1].Texture = soldat2;
			sprites[1].Scale = 1.0f;
			sprites[1].X = -480;
			sprites[1].Y = 45;
			sprites[1].Angle = 0;
			sprites[1].XAnchor = 0.5f;
			sprites[1].YAnchor = 0.5f;
			
			sprites[2] = new Sprite();
			sprites[2].Texture = soldat3;
			sprites[2].Scale = 1.0f;
			sprites[2].X = -480;
			sprites[2].Y = 45;
			sprites[2].Angle = 0;
			sprites[2].XAnchor = 0.5f;
			sprites[2].YAnchor = 0.5f;
			
			sprites[3] = new Sprite();
			sprites[3].Texture = soldat4;
			sprites[3].Scale = 1.0f;
			sprites[3].X = -480;
			sprites[3].Y = 45;
			sprites[3].Angle = 0;
			sprites[3].XAnchor = 0.5f;
			sprites[3].YAnchor = 0.5f;
			
			sprites[4] = new Sprite();
			sprites[4].Texture = soldat5;
			sprites[4].Scale = 1.0f;
			sprites[4].X = -480;
			sprites[4].Y = 45;
			sprites[4].Angle = 0;
			sprites[4].XAnchor = 0.5f;
			sprites[4].YAnchor = 0.5f;
			
			sprites[5] = new Sprite();
			sprites[5].Texture = soldat6;
			sprites[5].Scale = 1.0f;
			sprites[5].X = -480;
			sprites[5].Y = 45;
			sprites[5].Angle = 0;
			sprites[5].XAnchor = 0.5f;
			sprites[5].YAnchor = 0.5f;
			
			sprites[6] = new Sprite();
			sprites[6].Texture = soldat1r;
			sprites[6].Scale = 1.0f;
			sprites[6].X = -480;
			sprites[6].Y = 45;
			sprites[6].Angle = 0;
			sprites[6].XAnchor = 0.5f;
			sprites[6].YAnchor = 0.5f;
			
			sprites[7] = new Sprite();
			sprites[7].Texture = soldat2r;
			sprites[7].Scale = 1.0f;
			sprites[7].X = -480;
			sprites[7].Y = 45;
			sprites[7].Angle = 0;
			sprites[7].XAnchor = 0.5f;
			sprites[7].YAnchor = 0.5f;
			
			sprites[8] = new Sprite();
			sprites[8].Texture = soldat3r;
			sprites[8].Scale = 1.0f;
			sprites[8].X = -480;
			sprites[8].Y = 45;
			sprites[8].Angle = 0;
			sprites[8].XAnchor = 0.5f;
			sprites[8].YAnchor = 0.5f;
			
			sprites[9] = new Sprite();
			sprites[9].Texture = soldat4r;
			sprites[9].Scale = 1.0f;
			sprites[9].X = -480;
			sprites[9].Y = 45;
			sprites[9].Angle = 0;
			sprites[9].XAnchor = 0.5f;
			sprites[9].YAnchor = 0.5f;
			
			sprites[10] = new Sprite();
			sprites[10].Texture = soldat5r;
			sprites[10].Scale = 1.0f;
			sprites[10].X = -480;
			sprites[10].Y = 45;
			sprites[10].Angle = 0;
			sprites[10].XAnchor = 0.5f;
			sprites[10].YAnchor = 0.5f;
			
			sprites[11] = new Sprite();
			sprites[11].Texture = soldat6r;
			sprites[11].Scale = 1.0f;
			sprites[11].X = -480;
			sprites[11].Y = 45;
			sprites[11].Angle = 0;
			sprites[11].XAnchor = 0.5f;
			sprites[11].YAnchor = 0.5f;
			
			sprites[12] = new Sprite();
			sprites[12].Texture = fons;
			sprites[12].Scale = 1.0f;
			sprites[12].X = 0;
			sprites[12].Y = 0;
			sprites[12].Angle = 0;
			sprites[12].XAnchor = 0.5f;
			sprites[12].YAnchor = 0.5f;
			
			arma[0] = new Sprite();
			arma[0].Texture = armaTexture;
			arma[0].Scale = 1.0f;
			arma[0].X = -480;
			arma[0].Y = 45;
			arma[0].XAnchor = 0.5f;
			arma[0].YAnchor = 0.5f;
			
			arma[1] = new Sprite();
			arma[1].Texture = armaTexturer;
			arma[1].Scale = 1.0f;
			arma[1].X = -480;
			arma[1].Y = 45;
			arma[1].XAnchor = 0.5f;
			arma[1].YAnchor = 0.5f;
			
			for (int i=0; i<10; i++)
			{
				bales[i] = new Sprite();
				bales[i].Texture = bala;
				bales[i].Scale = 0.65f;
				bales[i].X = -10480;
				bales[i].Y = 10045;
				bales[i].XAnchor = 1.0f;
				bales[i].YAnchor = 0.5f;
			}
			for (int i=0; i<25; i++)//100000
			{
				demons[i] = new Sprite();
				demons[i].Texture = demon;
				demons[i].Scale = 1.0f;
				demons[i].X = 600;
				demons[i].Y = 600;
				demons[i].XAnchor = 0.5f;
				demons[i].YAnchor = 0.5f;
			}
			
			/*bales[1] = new Sprite();
			bales[1].Texture = balar;
			bales[1].Scale = 0.65f;
			bales[1].X = -100480;
			bales[1].Y = 100045;
			bales[1].XAnchor = 0.0f;
			bales[1].YAnchor = 0.5f;*/
			
			
			
			


			// set some colors and translucency
			//sprites[0].Color = Color.FromArgb( 127, 127, 255, 0 );
			//sprites[1].Color = Color.FromArgb( 127, 255, 0, 127 );
			//sprites[2].Color = Color.FromArgb( 127, 0, 127, 255 );

			// make a new camera
			camera = new Camera( screenwidth, screenheight );
			tempsAparicio=DateTime.Now.Second;

		}



		// --------------------------------------------------------------------
		// Device Event Handlers
		// --------------------------------------------------------------------
		protected virtual void InvalidateDeviceObjects( object sender, EventArgs e )
		{
		}
		
		protected virtual void RestoreDeviceObjects( object sender, EventArgs e )
		{
		}
		
		protected virtual void DeleteDeviceObjects( object sender, EventArgs e )
		{
		}
		
		protected virtual void EnvironmentResizing( object sender, CancelEventArgs e )
		{
			e.Cancel = true;
		}
		

		// --------------------------------------------------------------------
		// Process one iteration of the game loop
		// --------------------------------------------------------------------
		protected virtual void ProcessFrame()
		{
			// process the game only while it's not paused
			if( !paused )
			{
				// get the amount of time that passed since last frame
				float t = gametimer.Elapsed();


				// move camera
				//camera.X += (movingx * t);
				//camera.Y += (movingy * t);
				

				// rotate sprites at 1 radian/s counter-clockwise
				for( int i = 0; i < 12; i++ )
				{
					sprites[i].X += movingx* t;
					if(sprites[i].X<-480) //fem que no surti de la pantalla
						sprites[i].X=-480;
					if(sprites[i].X>480)
						sprites[i].X=480;
					
					sprites[i].Y += movingy* t;
					if(sprites[i].Y>220)
						sprites[i].Y=220;
					if(sprites[i].Y<-220)
						sprites[i].Y=-220;
					
				}
				for(int i=0; i<2; i++)
				{
					arma[i].X += movingx* t;
					if(arma[i].X<-480) //fem que no surti de la pantalla
						arma[i].X=-480;
					if(arma[i].X>480)
						arma[i].X=480;
					for (int k=0; k<12; k++)
					{
						arma[i].Y = sprites[k].Y; //+=movingy* t;
					}
					if(arma[i].Y>220)
						arma[i].Y=220;
					if(arma[i].Y<-220)
						arma[i].Y=-220;
				}
				if(pulsaRight==true)
				{
					if(escales==false)
					{
						movingx = 100.0f;
						if(soldatbo>5)
							soldatbo=0;
						soldatbo++;
						if(soldatbo==6)
							soldatbo=0;
						armabona=0;
					}
				}
				if(pulsaW==true)
				{
					if(arma[0].Angle<0.5f)
						arma[0].Angle+=0.1f;
					if(arma[1].Angle>-0.5f)
						arma[1].Angle-=0.1f;
				}
				if(pulsaS==true)
				{
					if(arma[0].Angle>-0.5f)
						arma[0].Angle-=0.1f;
					if(arma[1].Angle<0.5f)
						arma[1].Angle+=0.1f;
				}
				
				if(pulsaLeft==true)
				{
					if(escales==false)
					{
						movingx = -100.0f;
						if(soldatbo<6)
						{
							soldatbo=6;
						}
						soldatbo++;
						if(soldatbo==12)
							soldatbo=6;
						armabona=1;
					}
				}
				if(pulsaDown==true)
				{
					for(int i=0; i<12; i++)
					{
						if(sprites[i].X>=-104&&sprites[i].X<=-78&&sprites[i].Y<=250&&sprites[i].Y>=-110)
						{
							escales=true;
							movingy=100.0f;
						}
						if(sprites[i].X>=280&&sprites[i].X<=306&&sprites[i].Y<=196&&sprites[i].Y>=-172)
						{
							escales=true;
							movingy=100.0f;
						}
					}
				}
				if(pulsaDown==false&&pulsaUp==false)
				{
					movingy=0.0f;
				}
				if(pulsaUp==true)
				{
					
					
					
					for(int i=0; i<12; i++)
					{
						if(sprites[i].X>=-104&&sprites[i].X<=-78&&sprites[i].Y<=250&&sprites[i].Y>=-130)
						{
							escales=true;
							movingy=-100.0f;
						}
						if(sprites[i].X>=280&&sprites[i].X<=306&&sprites[i].Y<=196&&sprites[i].Y>=-172)
						{
							escales=true;
							movingy=-100.0f;
						}
						
						else
						{
							potSaltar=true;
							//movingy=-500.0f;
						}
						
						
					}
				}
				if(pulsaUp==false&&potSaltar&&antiVol)
				{
					for(int i=0; i<12; i++)
						sprites[i].Y+=-70.0f;
					potSaltar=false;
					antiVol=false;
				}
				if(pulsaUp==false&&pulsaDown==false)
				{
					
					movingy=0.0f;
				}
				if(pulsaEspai==false&&potDisparar)
				{
					bales[tirActual].X=sprites[soldatbo].X-10;
					bales[tirActual].Y=sprites[soldatbo].Y;
					bales[tirActual].Angle=arma[armabona].Angle;
					if(soldatbo==0||soldatbo==1||soldatbo==2||soldatbo==3||soldatbo==4||soldatbo==5)
					{
						bales[tirActual].XAnchor=1.0f;
						movingxbales[numVectors]=10.0f;
						//movingxbala = 10.0f;
					}
					if(soldatbo==6||soldatbo==7||soldatbo==8||soldatbo==9||soldatbo==10||soldatbo==11)
					{
						bales[tirActual].XAnchor=0.0f;
						movingxbales[numVectors]=-10.0f;
						//movingxbala = -10.0f;
					}
					movingybales[numVectors] = (float)Math.Tan(-arma[armabona].Angle)*(movingxbales[numVectors]);
					//movingybala = (float)Math.Tan(-arma[armabona].Angle)*movingxbala;
					tirActual++;
					if(tirActual==9)
						tirActual=0;
					numVectors++;
					if(numVectors==9)
						numVectors=0;
					
					potDisparar=false;					
				}
				if(pulsaEspai==true)
				{
					
					potDisparar=true;
					
				}
				
				for(int i=0; i<10; i++)
				{
					bales[i].X+=movingxbales[i];
					bales[i].Y+=movingybales[i];
				}
				//antixoc
				
				for (int i=0; i<10; i++)
				{
					if(bales[i].X<-361&&bales[i].Y>74)
						bales[i].X=60000;
					if(bales[i].X>-283&&bales[i].X<-130&&bales[i].Y<-50&&bales[i].Y>-110)
						bales[i].X=60000;
					if(bales[i].X>179&&bales[i].X<409&&bales[i].Y>197)
						bales[i].X=60000;
					if(bales[i].X>332&&bales[i].Y>14&&bales[i].Y<70)
						bales[i].X=60000;
					if(bales[i].X>332&&bales[i].Y>-172&&bales[i].Y<-110)
						bales[i].X=60000;
					if(bales[i].X>-52&&bales[i].X<102&&bales[i].Y>-110&&bales[i].Y<-50)
						bales[i].X=60000;
					if(bales[i].X>102&&bales[i].X<254&&bales[i].Y>-172&&bales[i].Y<12)
						bales[i].X=60000;
					
				}
				gravetat=true;
				for(int i=0; i<12; i++)
				{
					if(sprites[i].X>=-104&&sprites[i].X<=-78&&sprites[i].Y<=250&&sprites[i].Y>=-110||(sprites[i].X>=280&&sprites[i].X<=306&&sprites[i].Y<=196&&sprites[i].Y>=-172))
						gravetat=false;
				}
				
				if(pulsaUp==false&&pulsaDown==false&&gravetat==true)
					movingy=300.0f;
				
				for (int i=0; i<12; i++)
				{
					if(sprites[i].X<-350&&sprites[i].Y>=50&&evitarPujar==false)
					{
						sprites[i].Y=50;
						arma[armabona].Y=50;
						antiVol=true;
					}
					if(sprites[i].X>-360&&sprites[i].X<396&&sprites[i].Y==220)
					{
						sprites[i].Y=220;
						evitarPujar=true;
						antiVol=true;
					}
					if(sprites[i].X<-340&&sprites[i].Y==220)
					{
						sprites[i].X=-340;
						arma[1].X=-340;
						arma[0].X=-340;
						evitarPujar=true;
						antiVol=true;
					}
					evitarPujar=false;
					if(sprites[i].X>-283&&sprites[i].X<-130&&sprites[i].Y>-145&&sprites[i].Y<-120)
					{
						//movingy=0.0f;
						sprites[i].Y=-140;
						arma[0].Y=-140;
						arma[1].Y=-140;//-51
						antiVol=true;
					}
					if(sprites[i].X>-51&&sprites[i].X<100&&sprites[i].Y>-145&&sprites[i].Y<-120)
					{
						//movingy=0.0f;
						sprites[i].Y=-140;
						arma[0].Y=-140;
						arma[1].Y=-140;
						antiVol=true;
					}
					if(sprites[i].X>100&&sprites[i].X<253&&sprites[i].Y>-205&&sprites[i].Y<-180)
					{
						//movingy=0.0f;
						sprites[i].Y=-200;
						arma[0].Y=-200;
						arma[1].Y=-200;
						antiVol=true;
					}
					if(sprites[i].X>167&&sprites[i].Y==220)
					{
						sprites[i].X=168;
						arma[0].X=168;
						arma[1].X=168;
						antiVol=true;
					}
					if(sprites[i].X>=90&&sprites[i].Y==-140)
					{
						sprites[i].X=90;
						arma[0].X=90;
						arma[1].Y=90;
						antiVol=true;
					}
					if(sprites[i].X>168&&sprites[i].X<409&&sprites[i].Y>167&&sprites[i].Y<187)
					{
						//movingy=0.0f;
						sprites[i].Y=167;
						arma[0].Y=167;
						arma[1].Y=167;
						antiVol=true;
					}
					//if(sprites[i].X>332&&sprites[i].X<500&&sprites[i].Y>-200&&sprites[i].Y<-190)
					if(sprites[i].X>332&&sprites[i].X<500&&sprites[i].Y>-205&&sprites[i].Y<-180)
					{
						//movingy=0.0f;
						sprites[i].Y=-200;
						arma[0].Y=-200;
						arma[0].Y=-200;
						antiVol=true;
					}
					if(sprites[i].X>332&&sprites[i].X<500&&sprites[i].Y>-21&&sprites[i].Y<4)
					{
						//movingy=0.0f;
						sprites[i].Y=-16;
						arma[0].Y=-16;
						arma[0].Y=-16;
						antiVol=true;
					}
					if(sprites[i].X>409&&sprites[i].X<500&&sprites[i].Y==220)
					{
						//movingy=0.0f;
						sprites[i].Y=220;
						arma[0].Y=220;
						arma[0].Y=220;
						antiVol=true;
					}
				}
				if(DateTime.Now.Second==tempsAparicio+2)
				{
					if(demonActual<25)
					{
						tempsAparicio=(int)DateTime.Now.Second;
						demons[demonActual].X=485;
						demons[demonActual].Y=220;
					}
					//if(demonActual<25)
					demonActual++;
					if(demonActual>=25)
					{
						for (int k=0; k<25; k++)
						{
							if(demons[k].X>550)
							{
								tempsAparicio=(int)DateTime.Now.Second;
								demons[k].X=485;
								demons[k].Y=220;
								
							}
							
						}
						
					}
					
					/*if(demonActual>=25)
					{
						for(int k=1; k<=25; k++)
						{
							if(k==reciclar)
							{
								tempsAparicio=(int)DateTime.Now.Second;
								demons[reciclar-1].X=485;
								demons[reciclar-1].Y=220;
							}
						}
					}*/
					/*if(demonActual>=25)
					{
						for(int k=0; k<25; k++)
						{
							//reciclar=k;
							if(demons[k].X>550)
							{
								demons[k].X=485;
								demons[k].Y=220;
								k=24;
							}
						}
						
					}*/
					
				}
				if(DateTime.Now.Second==59)
				{
					tempsAparicio=0;
				}
				
				
				for(int k=0; k<25; k++)
				{
					if(demons[k].X>=-500&&demons[k].X<=500&&demons[k].Y>=-250&&demons[k].Y<=250)
					{
						if(demons[k].X>sprites[soldatbo].X)
						{
							demons[k].Texture=demonr;
							movingxdemons[k]=-60.0f;
						}
						if(demons[k].X<sprites[soldatbo].X)
						{
							demons[k].Texture=demon;
							movingxdemons[k]=60.0f;
						}
						if(demons[k].Y>sprites[soldatbo].Y)
							movingydemons[k]=-60.0f;
						if(demons[k].Y<sprites[soldatbo].Y)
							movingydemons[k]=60.0f;
						if(demons[k].Y>-5&&demons[k].Y<110&&demons[k].X>325)
						{
							movingydemons[k]=0.0f;
							movingxdemons[k]=-60.0f;
						}
						if(demons[k].Y>-197&&demons[k].Y<-80&&demons[k].X>325)
						{
							movingydemons[k]=0.0f;
							movingxdemons[k]=-60.0f;
						}
						if(demons[k].X>179&&demons[k].X<420&&demons[k].Y>167)
						{
							movingydemons[k]=-60.0f;
							movingxdemons[k]=0.0f;
						}
						if(demons[k].X<-349&&demons[k].Y>64)
						{
							movingydemons[k]=-60.0f;
							movingxdemons[k]=0.0f;
						}
						if(demons[k].X>-290&&demons[k].X<-115&&demons[k].Y>-135&&demons[k].Y<-25)
						{
							movingxdemons[k]=60.0f;
							movingydemons[k]=0.0f;
						}
						if(demons[k].X>-65&&demons[k].X<90&&demons[k].Y>-135&&demons[k].Y<-25)
						{
							movingxdemons[k]=-60.0f;
							movingydemons[k]=0.0f;
						}
						if(demons[k].X>90&&demons[k].X<265&&demons[k].Y>-197&&demons[k].Y<37)
						{
							movingxdemons[k]=-60.0f;
							movingydemons[k]=0.0f;
						}
						demons[k].X+=movingxdemons[k]*t;
						demons[k].Y+=movingydemons[k]*t;
						if(demons[k].X>=sprites[soldatbo].X-15&&demons[k].X<=sprites[soldatbo].X+15&&demons[k].Y>=sprites[soldatbo].Y-25&&demons[k].Y<=sprites[soldatbo].Y+25)
							Paused=true;
						
					}

				}
				/*for(int k=0; k<25; k++)
				{
					if(demons[k].Y>60&&demons[k].Y<110&&demons[k].X>332)
					{
						movingydemons[k]=0.0f;
						movingxdemons[k]=-60.0f;
					}
					demons[k].X+=movingxdemons[k]*t;
					demons[k].Y+=movingydemons[k]*t;
				}*/
				
				for(int k=0; k<10; k++)
				{
					for(int i=0; i<25; i++)
					{
						if(bales[k].X>=demons[i].X-25 && bales[k].X<=demons[i].X+25&&bales[k].Y>=demons[i].Y-25&&bales[k].Y<=demons[i].Y+25)
						{
							demons[i].X=600;
							bales[k].X=600;
							//reciclar++;
							//if(reciclar==25)
							//	reciclar=1;
							
						}
					}
				}
				
				/*demons[0].X+=movingxdemons*t;
				demons[0].Y+=movingydemons*t;
				if(demons[0].X>sprites[soldatbo].X)
				{
					demons[0].Texture=demonr;
					movingxdemons=-100.0f;
				}
				if(demons[0].X<sprites[soldatbo].X)
				{
					demons[0].Texture=demon;
					movingxdemons=100.0f;
				}
				if(demons[0].Y>sprites[soldatbo].Y)
					movingydemons=-100.0f;
				if(demons[0].Y<sprites[soldatbo].Y)
					movingydemons=100.0f;
				
				for(int k=0; k<10; k++)
				{
					if(bales[k].X>=demons[0].X-15 && bales[k].X<=demons[0].X+15&&bales[k].Y>=demons[0].Y-15&&bales[k].Y     <=demons[0].Y+15)
						demons[0].X=10000;
					
				}*/
				
			}
			else
			{
				System.Threading.Thread.Sleep( 1 );
			}
		}
		


		// --------------------------------------------------------------------
		// Render the current game screen
		// --------------------------------------------------------------------
		protected virtual void Render()
		{
			if( graphics != null )
			{
				// check to see if the device has been lost. If so, try to get
				// it back.
				if( graphicslost )
				{
					try
					{
						graphics.TestCooperativeLevel();
					}
					catch( Direct3D.DeviceLostException )
					{
						// device cannot be reaquired yet, just return
						return;
					}
					catch( Direct3D.DeviceNotResetException )
					{
						// device has not been reset, but it can be reaquired now

						graphics.Reset( graphics.PresentationParameters );
					}
					graphicslost = false;
				}


				try
				{

					graphics.Clear( Direct3D.ClearFlags.Target, Color.White , 1.0f, 0 );
					graphics.BeginScene();

					// start rendering
					spriterenderer.Begin();

					// render the sprites in reverse order (looks better for this demo)
					/*for( int i = numsprites - 1; i >= 0; i-- )
					{
						sprites[i].Draw( spriterenderer, camera );
					}*/
					/*{
						sprites[6].Draw(spriterenderer, camera); //fons
						
						if(sprites[0].X==0)
							sprites[0].Draw(spriterenderer, camera); //primera imatge
						if(sprites[0].X<=100&&sprites[0].X>0)
							sprites[1].Draw(spriterenderer, camera);
						if(sprites[1].X<=200&&sprites[1].X>100)
							sprites[2].Draw(spriterenderer, camera);
						if(sprites[2].X<=300&&sprites[2].X>200)
							sprites[3].Draw(spriterenderer, camera);
						if(sprites[3].X<=400&&sprites[3].X>300)
							sprites[4].Draw(spriterenderer, camera);
						if(sprites[4].X<=500&&sprites[4].X>400)
							sprites[5].Draw(spriterenderer, camera);
					}*/
					sprites[12].Draw(spriterenderer, camera); //fons
					for(int k=0; k<10; k++)
					{
						bales[k].Draw(spriterenderer,camera);
					}
					sprites[soldatbo].Draw(spriterenderer, camera);
					arma[armabona].Draw(spriterenderer, camera);
					for (int i=0; i<25; i++)
					{
						demons[i].Draw(spriterenderer, camera);
						
					}
					//demons[0].Draw(spriterenderer,camera);
					
					
					// end rendering
					spriterenderer.End();
					
					graphics.EndScene();
					graphics.Present();
				}

				// device has been lost, and it cannot be re-initialized yet
				catch( Direct3D.DeviceLostException )
				{
					graphicslost = true;
				}
			}
		}




		// --------------------------------------------------------------------
		// Run the game
		// --------------------------------------------------------------------
		public void Run()
		{
			// reset the game timer
			gametimer.Reset();

			// loop while form is valid
			while( this.Created )
			{
				// process one frame of the game
				ProcessFrame();

				// render the current scene
				Render();

				// handle all events
				Application.DoEvents();
			}
		}
		

		// --------------------------------------------------------------------
		// Handle windows events
		// --------------------------------------------------------------------

		
		protected override void OnLostFocus( EventArgs e )
		{
			base.OnLostFocus( e );
			Paused = true;
		}

		

		protected override void OnKeyDown( KeyEventArgs e )
		{
			base.OnKeyDown( e );

			if( e.KeyCode == System.Windows.Forms.Keys.Escape )
			{
				this.Close();
			}

			if ( e.KeyCode == System.Windows.Forms.Keys.P )
			{
				Paused = !Paused;
			}

			// move at 100 pixels per second if W,S,A, or D is pressed
			if( e.KeyCode == System.Windows.Forms.Keys.Up ) //{ movingy = -100.0f; }
			{
				pulsaUp=true;
				//movingy=-100.0f;
				/*for(int i=0; i<12; i++)
				{
					if(sprites[i].X>=-104&&sprites[i].X<=-78&&sprites[i].Y<=250&&sprites[i].Y>=-110)
					{
						escales=true;
						movingy=-100.0f;
					}
					if(sprites[i].X>=280&&sprites[i].X<=306&&sprites[i].Y<=196&&sprites[i].Y>=-172)
					{
						escales=true;
						movingy=-100.0f;
					}
				}*/
			}
			
			if( e.KeyCode == System.Windows.Forms.Keys.Down )
			{
				pulsaDown=true;
				/*
				for(int i=0; i<12; i++)
				{
					if(sprites[i].X>=-104&&sprites[i].X<=-78&&sprites[i].Y<=250&&sprites[i].Y>=-110)
					{
						escales=true;
						movingy=100.0f;
					}
					if(sprites[i].X>=280&&sprites[i].X<=306&&sprites[i].Y<=196&&sprites[i].Y>=-172)
					{
						escales=true;
						movingy=100.0f;
					}
				}*/
			}
			if( e.KeyCode == System.Windows.Forms.Keys.Left )
			{
				pulsaLeft=true;
				/*if(escales==false)
				{
					movingx = -100.0f;
					if(soldatbo<6)
					{
						soldatbo=6;
					}
					soldatbo++;
					if(soldatbo==12)
						soldatbo=6;
					armabona=1;
				}*/
			}
			if( e.KeyCode == System.Windows.Forms.Keys.Right )
			{
				pulsaRight=true;
				/*
				if(escales==false)
				{
					movingx = 100.0f;
					if(soldatbo>5)
						soldatbo=0;
					soldatbo++;
					if(soldatbo==6)
						soldatbo=0;
					armabona=0;*/
			}
			
			if( e.KeyCode == System.Windows.Forms.Keys.W)
			{
				pulsaW=true;
				/*
			if(arma[armabona].Angle<0.5f)
				arma[armabona].Angle+=0.1f;*/
				
			}
			if( e.KeyCode == System.Windows.Forms.Keys.S)
			{
				pulsaS=true;
				/*if(arma[armabona].Angle>-0.5f)
					arma[armabona].Angle-=0.1f;*/
			}
			if( e.KeyCode == System.Windows.Forms.Keys.Space)
			{
				pulsaEspai=true;
			}
		}
		
		


		protected override void OnKeyUp( KeyEventArgs e )
		{
			base.OnKeyUp( e );


			// stop moving if keys are released
			if( e.KeyCode == System.Windows.Forms.Keys.Up )
			{
				escales=false;
				//movingy = 0.0f;
				pulsaUp=false;
			}
			if( e.KeyCode == System.Windows.Forms.Keys.Down )
			{
				escales=false;
				//movingy = 0.0f;
				pulsaDown=false;
			}
			if( e.KeyCode == System.Windows.Forms.Keys.Left ) {pulsaLeft=false; movingx = 0.0f;}//{ movingx = 0.0f; }
			if( e.KeyCode == System.Windows.Forms.Keys.Right ) {pulsaRight=false; movingx = 0.0f;}//{ movingx = 0.0f; }
			if( e.KeyCode == System.Windows.Forms.Keys.W) {pulsaW=false;}
			if(	e.KeyCode == System.Windows.Forms.Keys.S) {pulsaS=false;}
			if( e.KeyCode == System.Windows.Forms.Keys.Space) {pulsaEspai=false;}
		}

		// --------------------------------------------------------------------
		// Property to pause/unpause the game, or get its pause state
		// --------------------------------------------------------------------
		public bool Paused
		{
			get { return paused; }
			set
			{
				// pause the game
				if( value == true && paused == false )
				{
					gametimer.Pause();
					paused = true;
				}

				// unpause the game
				if( value == false && paused == true )
				{
					gametimer.Unpause();
					paused = false;
				}
			}
		}


		// --------------------------------------------------------------------
		// Entry point of the program, creates a new game and runs it
		// --------------------------------------------------------------------
		static void Main()
		{
			Game game;
			try
			{
				game = new Game();
				game.InitializeGraphics();
				game.InitializeSound();
				game.InitializeInput();
				game.InitializeResources();

				game.Show();
				game.Run();
			}
			catch( Exception e )
			{
				MessageBox.Show( "Error: " + e.Message );
			}
		}
	}
}
