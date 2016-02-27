/*Copyright Yura Bilyk, 2015*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media;
using System.Windows.Input;
using System.Windows;

using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RootFinder
{
    public class DrawingVisualHost : FrameworkElement
    {
        public Visual _child;
        //public DrawingVisualHost()
        //{
        //    //theVisuals = new VisualCollection(this);
        //   // this.Loaded += (s,e) => { theVisuals.Add(DrawGraph(-1, 1, 1, 1));  };
            
        //   // theVisuals.Add(CreateDrawingVisualRectangle());
        //}

        public Visual Child
        {
            get
            {
                return _child;
            }

            set
            {
                if (_child != null)
                {
                    RemoveVisualChild(_child);
                }

                _child = value;

                if (_child != null)
                {
                    AddVisualChild(_child);
                }
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return _child != null ? 1 : 0;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (_child!=null && index==0)
            {
                return _child;
                
            }
            else throw new ArgumentOutOfRangeException();
        }

    }


    public class VisualTargetPresentationSource : PresentationSource
    {
        public event EventHandler OnVisualRootChange = null; //notify when visual root updated
        private VisualTarget _visualTarget;
        public VisualTargetPresentationSource(HostVisual hostVisual)
        {
            _visualTarget = new VisualTarget(hostVisual);
        }


        public override Visual RootVisual
        {
            get
            {
                return _visualTarget.RootVisual;
            }

            set
            {
                Visual oldRoot = _visualTarget.RootVisual;

                // Set the root visual of the VisualTarget.  This visual will
                // now be used to visually compose the scene.

                _visualTarget.RootVisual = value;

                // Tell the PresentationSource that the root visual has
                // changed.  This kicks off a bunch of stuff like the
                // Loaded event.

                RootChanged(oldRoot, value);
                // Kickoff layout...
                UIElement rootElement = value as UIElement;

                if (rootElement != null)
                {
                    rootElement.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    rootElement.Arrange(new Rect(rootElement.DesiredSize));
                }
                if (OnVisualRootChange != null)
                    OnVisualRootChange(this, EventArgs.Empty);
            }
        }

        protected override CompositionTarget GetCompositionTargetCore()
        {
            return _visualTarget;
        }

        public override bool IsDisposed
        {
            get
            {
                // We don't support disposing this object.
                return false;
            }
        }
        
    }
}
