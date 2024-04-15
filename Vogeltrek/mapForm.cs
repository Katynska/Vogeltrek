﻿using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Forms;

namespace Vogeltrek
{
    public partial class mapForm : Form
    {
        private GMapOverlay markersOverlay;
        public mapForm()
        {
            InitializeComponent();
            markersOverlay = new GMapOverlay("markers");
        }

        private void btnDisplay_Click(object sender, EventArgs e)
        {
            string coordinate = townComboBox.SelectedItem.ToString();
            string[] parts = coordinate.Split(',');
            double latitude = double.Parse(parts[0], CultureInfo.InvariantCulture);
            double longitude = double.Parse(parts[1], CultureInfo.InvariantCulture);

            AddMarker(latitude, longitude);
        }
        private void AddMarker(double latitude, double longitude)
        {
            GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(latitude, longitude), GMarkerGoogleType.red);
            markersOverlay.Markers.Add(marker);
            gMapControl1.Overlays.Add(markersOverlay);
            gMapControl1.Position = new PointLatLng(latitude, longitude);
            gMapControl1.Zoom = 30;
        }

        private void gMapControl1_Load(object sender, EventArgs e)
        {
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
            gMapControl1.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            gMapControl1.MinZoom = 2;
            gMapControl1.MaxZoom = 16;
            gMapControl1.Zoom = 4;
            gMapControl1.Position = new GMap.NET.PointLatLng(66.4169575018027, 94.25025752215694);
            gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            gMapControl1.CanDragMap = true;
            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.ShowCenter = false;
            gMapControl1.ShowTileGridLines = false;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            markersOverlay.Markers.Clear();
        }

    }
}
