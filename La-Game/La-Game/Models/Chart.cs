﻿using System.Collections.Generic;

namespace La_Game.Models
{
    public class Chart
    {
        public string[] labels { get; set; }
        public List<Datasets> datasets { get; set; }
    }
    public class Datasets
    {
        public string label { get; set; }
        public string backgroundColor { get; set; }
        public string borderColor { get; set; }
        public string hoverBackgroundColor { get; set; }
        public string borderWidth { get; set; }
        public bool fill { get; set; }
        public double[] data { get; set; }
    }
}