﻿using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HumanUI.Components
{
    public class AddElementsToShape : GH_Component
    {
        public AddElementsToShape() : base("Add Elements to Shape(s)","AddElem2Shape","Put UI Elements (Like text!) over the top of a shape/shapes element","Human UI","UI Output")
        {

        }
        public override Guid ComponentGuid => new Guid("{DB39D16E-1BE9-41C5-B69F-7B52F38E1EF0}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Shape to add to", "S", "The Shape object to add text to", GH_ParamAccess.item);
            pManager.AddTextParameter("Element to add", "E", "The element(s) to add to the shapes", GH_ParamAccess.list);
            pManager.AddPointParameter("Element Location", "L", "The point at which to position the element", GH_ParamAccess.list);
            pManager.AddNumberParameter("Scale", "Sc", "The scale (this should match whatever you're using for your shape)", GH_ParamAccess.item, 1);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
           
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            object ShapeObject = null;
            List<UIElement_Goo> elemsToAdd = new List<UIElement_Goo>();
            var elemPoints = new List<Point3d>();
            double scale = 1.0;


            if (!DA.GetData("Shape to add to", ref ShapeObject)) return;
            if (!DA.GetDataList("Element to Add", elemsToAdd)) return;
            if (!DA.GetDataList("Text Location", elemPoints)) return;
            if (!DA.GetData("Scale", ref scale)) return;

            Grid G = HUI_Util.GetUIElement<ClickableShapeGrid>(ShapeObject);
            if(G == null)
            {
                G = HUI_Util.GetUIElement<Grid>(ShapeObject);
            }
            if (G == null) {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to get shape");
                return;
            }

            //remove all non-shape children
           G.Children.OfType<FrameworkElement>()
                .Where(c => !(c is Shape)).ToList()
                .ForEach(g => G.Children.Remove(g));

            var p = new Rhino.Geometry.Polyline(elemPoints);

            //flip to "shape coordinates"
            p.Transform(Rhino.Geometry.Transform.Mirror(Plane.WorldZX));
            //scale about the world origin
            p.Transform(Rhino.Geometry.Transform.Scale(Point3d.Origin, scale));


            for (int i = 0; i < elemsToAdd.Count; i++)
            {
                UIElement_Goo u = elemsToAdd[i];
                var location = p[i];
                
                //make sure it doesn't already have a parent
                HUI_Util.removeParent(u.element);



            }

        }
    }
}
