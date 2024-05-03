using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;


namespace VogeltrekWPF.Scripts
{
    internal class GmapSheet
    {
        public static void ConfigureMap(GMapControl map)
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            // choose your provider here
            map.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance;
            map.MinZoom = 2;
            map.MaxZoom = 17;
            // whole world zoom
            map.Zoom = 2;
            // lets the map use the mousewheel to zoom
            map.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            // lets the user drag the map
            map.CanDragMap = true;
            // lets the user drag the map with the left mouse button
            map.DragButton = MouseButton.Left;
        }
    }
}
