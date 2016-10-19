using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Windows;
using System.Windows.Media;
using Cloudmaster.WCS.WebApps.Cleaning.Models;
using WCS.Services.DataServices;


namespace HTML5.WPFPathsDemo.Helpers
{
    public static class HtmlHelperExtensions
    {
#if DEBUG
        private static bool includeCarriageReturns = true;
#else
        private static bool includeCarriageReturns = false;
#endif


        public static MvcHtmlString DrawFloorplanReadyForCleaning(this HtmlHelper html, string target, string backgroundUri, IList<CleaningTableSummaryRow> rows, int width, int height)
        {
            // String Builders

            var scriptBuilder = new StringBuilder();
            var drawContentFunctionBuilder = new StringBuilder();

            // Obfuscited Names

            string obfuscatedBackgroundImage = GetObfuscatedBackgroundImageName(target);
            string obfuscatedContextName = GetObfuscatedContextName(target);

            // Open draw Content Function

            drawContentFunctionBuilder.AppendFormat(@"function draw{0}() {{", target);
            if (includeCarriageReturns) { drawContentFunctionBuilder.AppendLine(); }

            drawContentFunctionBuilder.AppendFormat(@"{0}.clearRect(0, 0, {1}, {2});", obfuscatedContextName, width, height);
            if (includeCarriageReturns) { drawContentFunctionBuilder.AppendLine(); }

            drawContentFunctionBuilder.AppendFormat(@"draw{0}();", obfuscatedBackgroundImage);
            if (includeCarriageReturns) { drawContentFunctionBuilder.AppendLine(); }

            foreach (var row in rows)
            {
                string drawFunctionName = GetDrawFunctionNameForLayer(target, row.Key);

                if ((row.Bed.CurrentStatus == BedStatus.Dirty) && (row.IsAvailableNow | !row.IsOccupied))
                {
                    drawContentFunctionBuilder.AppendFormat(@"{0}('#FF0000');", drawFunctionName);
                    if (includeCarriageReturns) { drawContentFunctionBuilder.AppendLine(); }
                }
                else if ((row.Bed.CurrentStatus == BedStatus.RequiresDeepClean) && (row.IsAvailableNow | !row.IsOccupied))
                {
                    drawContentFunctionBuilder.AppendFormat(@"{0}('#653EA0');", drawFunctionName);
                    if (includeCarriageReturns) { drawContentFunctionBuilder.AppendLine(); }
                }
            }

            // Close Functions

            drawContentFunctionBuilder.Append("}");
            if (includeCarriageReturns) { drawContentFunctionBuilder.AppendLine(); }

            // Append Scripts to One 

            scriptBuilder.Append(drawContentFunctionBuilder);

            return MvcHtmlString.Create(scriptBuilder.ToString());
        }

        public static MvcHtmlString DrawFloorplanCleaningStatus(this HtmlHelper html, string target, string backgroundUri, IList<CleaningTableSummaryRow> rows, int width, int height)
        {
            // String Builders

            var scriptBuilder = new StringBuilder();
            var drawContentFunctionBuilder = new StringBuilder();

            // Obfuscited Names

            string obfuscatedBackgroundImage = GetObfuscatedBackgroundImageName(target);
            string obfuscatedContextName = GetObfuscatedContextName(target);

            // Open draw Content Function

            drawContentFunctionBuilder.AppendFormat(@"function draw{0}() {{", target);
            if (includeCarriageReturns) { drawContentFunctionBuilder.AppendLine(); }

            drawContentFunctionBuilder.AppendFormat(@"{0}.clearRect(0, 0, {1}, {2});", obfuscatedContextName, width, height);
            if (includeCarriageReturns) { drawContentFunctionBuilder.AppendLine(); }

            drawContentFunctionBuilder.AppendFormat(@"draw{0}();", obfuscatedBackgroundImage);
            if (includeCarriageReturns) { drawContentFunctionBuilder.AppendLine(); }

            foreach (var row in rows)
            {
                string drawFunctionName = GetDrawFunctionNameForLayer(target, row.Key);

                if (row.Bed.CurrentStatus == BedStatus.Clean) 
                {
                    //drawContentFunctionBuilder.AppendFormat(@"{0}('#00FF00');", drawFunctionName);
                    //if (includeCarriageReturns) { drawContentFunctionBuilder.AppendLine(); }
                }
                if (row.Bed.CurrentStatus == BedStatus.Dirty) 
                {
                    drawContentFunctionBuilder.AppendFormat(@"{0}('#FF0000');", drawFunctionName);
                    if (includeCarriageReturns) { drawContentFunctionBuilder.AppendLine(); }
                }
                else if (row.Bed.CurrentStatus == BedStatus.RequiresDeepClean)
                {
                    drawContentFunctionBuilder.AppendFormat(@"{0}('#653EA0');", drawFunctionName);
                    if (includeCarriageReturns) { drawContentFunctionBuilder.AppendLine(); }
                }
            }

            // Close Functions

            drawContentFunctionBuilder.Append("}");
            if (includeCarriageReturns) { drawContentFunctionBuilder.AppendLine(); }

            // Append Scripts to One 

            scriptBuilder.Append(drawContentFunctionBuilder);

            return MvcHtmlString.Create(scriptBuilder.ToString());
        }


        public static MvcHtmlString DrawFloorplan(this HtmlHelper html, string resourceDictionaryUrl, string target, string backgroundUri, string defaultColor, IList<CleaningTableSummaryRow> rows, int width, int height)
        {
#if !DEBUG 
            try
            {
#endif
                var resourceDictionary = (ResourceDictionary)System.Windows.Markup.XamlReader.Load(System.Xml.XmlReader.Create(GetResourceDictionaryUri(resourceDictionaryUrl)));

                var scriptBuilder = new StringBuilder();
                var variablesBuilder = new StringBuilder();
                var onReadyBuilder = new StringBuilder();
                var drawContentFunctionBuilder = new StringBuilder();
                var drawContentFunctionsBuilder = new StringBuilder();
                var onMouseMoveBuilder = new StringBuilder();

                string obfuscatedBackgroundImage = GetObfuscatedBackgroundImageName(target);
                string obfuscatedCanvasName = GetObfuscatedCanvasName(target);
                string obfuscatedContextName = GetObfuscatedContextName(target);
                string obfuscatedSelectedLayerVariableName = GetObfuscatedSelectedLayerVariableName(target);

                // Define variables

                variablesBuilder.AppendFormat("var {0};", obfuscatedBackgroundImage);
                if (includeCarriageReturns) { variablesBuilder.AppendLine(); }
                variablesBuilder.AppendFormat("var {0};", obfuscatedContextName);
                if (includeCarriageReturns) { variablesBuilder.AppendLine(); }
                variablesBuilder.AppendFormat("var {0};", obfuscatedSelectedLayerVariableName);
                if (includeCarriageReturns) { variablesBuilder.AppendLine(); }

                // Open OnReady Function

                onReadyBuilder.Append("$(document).ready(function () { ");
                if (includeCarriageReturns) { onReadyBuilder.AppendLine(); }

                // Create Canvas and Context in OnReady

                onReadyBuilder.AppendFormat(@"var {0} = $('<canvas id=""{0}"" width=""{1}"" height=""{2}"" style=""width:{1}px; height:{2}px; border:0px solid #000000;"" />');", obfuscatedCanvasName, width, height);
                if (includeCarriageReturns) { onReadyBuilder.AppendLine(); }

                onReadyBuilder.AppendFormat("$('#{0}').append({1});", target, obfuscatedCanvasName);
                if (includeCarriageReturns) { onReadyBuilder.AppendLine(); }

                onReadyBuilder.AppendFormat(@"var canvas = document.getElementById(""{0}"");", obfuscatedCanvasName);
                if (includeCarriageReturns) { onReadyBuilder.AppendLine(); }

                onReadyBuilder.AppendFormat(@"{0} = canvas.getContext(""2d"");", obfuscatedContextName);
                if (includeCarriageReturns) { onReadyBuilder.AppendLine(); }

                // Add mouse move event handler to OnReady

                //onReadyBuilder.AppendFormat(@"$('#{0}').mousedown(function (e)  {{  var x = e.pageX - $(this).offset().left; var y = e.pageY - $(this).offset().top; onMouseDown(x, y); }});", obfuscatedCanvasName);
                //if (includeCarriageReturns) { onReadyBuilder.AppendLine(); }

                // Open draw Content Function

                drawContentFunctionBuilder.AppendFormat(@"function draw{0}() {{", target);
                if (includeCarriageReturns) { drawContentFunctionBuilder.AppendLine(); }

                drawContentFunctionBuilder.AppendFormat(@"{0}.clearRect(0, 0, {1}, {2});", obfuscatedContextName, width, height);
                if (includeCarriageReturns) { drawContentFunctionBuilder.AppendLine(); }

                drawContentFunctionBuilder.AppendFormat(@"draw{0}();", obfuscatedBackgroundImage);
                if (includeCarriageReturns) { drawContentFunctionBuilder.AppendLine(); }

                // Open on mouse move

                onMouseMoveBuilder.Append("function onMouseDown(x, y) {");
                if (includeCarriageReturns) { onMouseMoveBuilder.AppendLine(); }

                onMouseMoveBuilder.AppendFormat(@"var canvas = document.getElementById(""{0}"");", obfuscatedCanvasName);
                if (includeCarriageReturns) { onMouseMoveBuilder.AppendLine(); }

                onMouseMoveBuilder.AppendFormat(@"context = canvas.getContext(""2d"");");
                if (includeCarriageReturns) { onMouseMoveBuilder.AppendLine(); }
                
                // Create Background

                CreateBackground(onReadyBuilder, drawContentFunctionBuilder, drawContentFunctionsBuilder, onMouseMoveBuilder, backgroundUri, target, width, height, 1);

                // Included Layers

                rows.ToList().ForEach(row =>
                {
                    var layer = (DrawingBrush)resourceDictionary[row.Key];

                    CreateLayer(variablesBuilder, onReadyBuilder, drawContentFunctionBuilder, drawContentFunctionsBuilder, onMouseMoveBuilder, layer, row.Key, target, width, height, defaultColor, 1, 0.1, true, 1);
                });

                // Close Functions

                onReadyBuilder.Append("});");
                if (includeCarriageReturns) { onReadyBuilder.AppendLine(); }
                onMouseMoveBuilder.Append("}");
                if (includeCarriageReturns) { onMouseMoveBuilder.AppendLine(); }
                drawContentFunctionBuilder.Append("}");
                if (includeCarriageReturns) { drawContentFunctionBuilder.AppendLine(); }

                scriptBuilder.Append(variablesBuilder);
                scriptBuilder.Append(onReadyBuilder);
                //scriptBuilder.Append(drawContentFunctionBuilder);
                scriptBuilder.Append(drawContentFunctionsBuilder);
                //scriptBuilder.Append(onMouseMoveBuilder);

                return MvcHtmlString.Create(scriptBuilder.ToString());
#if !DEBUG 
            }
            catch (Exception ex)
            {
                return MvcHtmlString.Create(ex.Message.ToString());
            }
#endif
        }


        private static void CreateBackground(StringBuilder onReadyBuilder, StringBuilder drawContentFunctionBuilder, StringBuilder drawContentFunctionsBuilder, StringBuilder onMouseMoveBuilder, string backgroundUri, string target, int width, int height, double scale)
        {
            double scaledWidth = width * scale;
            double scaledHeight = height * scale;

            string obfuscatedBackgroundImage = GetObfuscatedBackgroundImageName(target);
            string obfuscatedCanvasName = GetObfuscatedCanvasName(target);
            string obfuscatedContextName = GetObfuscatedContextName(target);
            
            // Add Draw Function

            drawContentFunctionsBuilder.AppendFormat(@"function draw{0}() {{", obfuscatedBackgroundImage);
            if (includeCarriageReturns) { drawContentFunctionsBuilder.AppendLine(); }
            drawContentFunctionsBuilder.AppendFormat("{0}.drawImage({1}, 0, 0, {2}, {3});", obfuscatedContextName, obfuscatedBackgroundImage, scaledWidth, scaledHeight);
            if (includeCarriageReturns) { drawContentFunctionsBuilder.AppendLine(); }
            drawContentFunctionsBuilder.AppendLine("}");
            if (includeCarriageReturns) { drawContentFunctionsBuilder.AppendLine(); }


            //onReadyBuilder.AppendLine("var devicePixelRatio = window.devicePixelRatio || 1;");
            //onReadyBuilder.AppendFormat("var backingStoreRatio = {0}.webkitBackingStorePixelRatio || {0}.mozBackingStorePixelRatio || {0}.msBackingStorePixelRatio || {0}.oBackingStorePixelRatio || {0}.backingStorePixelRatio || 1;", obfuscatedContextName);
            //onReadyBuilder.AppendLine("alert(devicePixelRatio / backingStoreRatio);");

            // Add to OnReady
            
            string backgroundImage = GetResourceDictionaryUri(backgroundUri);

            onReadyBuilder.AppendFormat("{0} = new Image();", obfuscatedBackgroundImage);
            if (includeCarriageReturns) { onReadyBuilder.AppendLine(); }
            onReadyBuilder.AppendFormat(@"{0}.onload = function() {{", obfuscatedBackgroundImage);
            if (includeCarriageReturns) { onReadyBuilder.AppendLine(); }
            onReadyBuilder.AppendFormat("draw{0}();", target);
            if (includeCarriageReturns) { onReadyBuilder.AppendLine(); }

            onReadyBuilder.AppendLine("};");
            if (includeCarriageReturns) { onReadyBuilder.AppendLine(); }
            onReadyBuilder.AppendFormat("{0}.src = '{1}';", obfuscatedBackgroundImage, backgroundImage);
            if (includeCarriageReturns) { onReadyBuilder.AppendLine(); }
        }

        private static void CreateLayer(StringBuilder variablesBuilder, StringBuilder onReadyBuilder, StringBuilder drawContentFunctionBuilder, StringBuilder drawContentFunctionsBuilder, StringBuilder onMouseMoveBuilder, DrawingBrush layer, string key, string target, int width, int height, string fillStyle, double alpha, double lineWidth, bool includeInHitTest, double scale)
        {
            // Create Canvas

            string drawFunctionName = GetDrawFunctionNameForLayer(target, key);
            string obfuscatedCanvasName = GetObfuscatedCanvasName(target);
            string obfuscatedContextName = GetObfuscatedContextName(target);
            string obfuscatedSelectedLayerVariableName = GetObfuscatedSelectedLayerVariableName(target);

            // Add to draw function

            drawContentFunctionBuilder.AppendFormat(@"if ({0} == '{1}') {{ {2}('{3}'); }} else {{ {2}('{4}'); }}", obfuscatedSelectedLayerVariableName, key,drawFunctionName, "#FAFAFA", fillStyle);
            if (includeCarriageReturns) { drawContentFunctionBuilder.AppendLine(); }

            // Open Draw Function

            drawContentFunctionsBuilder.AppendFormat(@"function {0} (fillStyle) {{", drawFunctionName);
            if (includeCarriageReturns) { drawContentFunctionsBuilder.AppendLine(); }
            
            // Set Properties 

            drawContentFunctionsBuilder.AppendFormat("{0}.globalAlpha = {1};", obfuscatedContextName, alpha);
            if (includeCarriageReturns) { drawContentFunctionsBuilder.AppendLine(); }

            drawContentFunctionsBuilder.AppendFormat(@"{0}.fillStyle = fillStyle;", obfuscatedContextName);
            if (includeCarriageReturns) { drawContentFunctionsBuilder.AppendLine(); }

            drawContentFunctionsBuilder.AppendFormat(@"{0}.lineWidth = {1};", obfuscatedContextName, lineWidth);
            if (includeCarriageReturns) { drawContentFunctionsBuilder.AppendLine(); }
            drawContentFunctionsBuilder.AppendFormat(@"{0}.strokeStyle = 'black';", obfuscatedContextName);
            if (includeCarriageReturns) { drawContentFunctionsBuilder.AppendLine(); }


            if (layer == null)
            {
                drawContentFunctionsBuilder.AppendLine(@"// No drawing information found");
            }
            else
            {
                var group = (DrawingGroup)layer.Drawing;

                if (group != null)
                {
                    var geometryDrawings = group.Children.OfType<GeometryDrawing>().ToList();

                    geometryDrawings.ForEach(g =>
                    {
                        var geomtery = (Geometry)g.Geometry;

                        var pathGeomtery = geomtery.GetFlattenedPathGeometry();

                        pathGeomtery.Figures.OfType<PathFigure>().ToList().ForEach(f =>
                        {
                            f.Segments.ToList().ForEach(s =>
                            {
                                if (s.GetType() == typeof(PolyLineSegment))
                                {
                                    var polyLineSegment = (PolyLineSegment)s;

                                    var first = polyLineSegment.Points.First();

                                    drawContentFunctionsBuilder.AppendFormat("{0}.beginPath();", obfuscatedContextName);
                                    if (includeCarriageReturns) { drawContentFunctionsBuilder.AppendLine(); }

                                    onMouseMoveBuilder.AppendFormat("context.beginPath();");
                                    if (includeCarriageReturns) { onMouseMoveBuilder.AppendLine(); }

                                    double firstX = first.X * scale;
                                    double firstY = first.Y * scale;

                                    drawContentFunctionsBuilder.AppendFormat("{0}.moveTo({1:0}, {2:0});", obfuscatedContextName, firstX, firstY);
                                    if (includeCarriageReturns) { drawContentFunctionsBuilder.AppendLine(); }

                                    onMouseMoveBuilder.AppendFormat("context.moveTo({0:0}, {1:0});", firstX, firstY);
                                    if (includeCarriageReturns) { onMouseMoveBuilder.AppendLine(); }

                                    polyLineSegment.Points.ToList().ForEach(p =>
                                    {
                                        if (!p.Equals(first))
                                        {
                                            double pX = p.X * scale;
                                            double pY = p.Y * scale;

                                            drawContentFunctionsBuilder.AppendFormat("{0}.lineTo({1:0}, {2:0});", obfuscatedContextName, pX, pY);
                                            if (includeCarriageReturns) { drawContentFunctionsBuilder.AppendLine(); }

                                            onMouseMoveBuilder.AppendFormat("context.lineTo({0:0}, {1:0});", pX, pY);
                                            if (includeCarriageReturns) { onMouseMoveBuilder.AppendLine(); }
                                        }
                                    });

                                    // Close Path and Fill

                                    drawContentFunctionsBuilder.AppendFormat("{0}.closePath();", obfuscatedContextName);
                                    if (includeCarriageReturns) { drawContentFunctionsBuilder.AppendLine(); }

                                    drawContentFunctionsBuilder.AppendFormat("{0}.fill();", obfuscatedContextName);
                                    if (includeCarriageReturns) { drawContentFunctionsBuilder.AppendLine(); }

                                    drawContentFunctionsBuilder.AppendFormat(@"{0}.stroke();", obfuscatedContextName);
                                    if (includeCarriageReturns) { drawContentFunctionsBuilder.AppendLine(); }

                                    // Close Mouse Path and Check if Point Is in Path

                                    onMouseMoveBuilder.AppendFormat("context.closePath();", obfuscatedContextName);
                                    if (includeCarriageReturns) { onMouseMoveBuilder.AppendLine(); }

                                    // onMouseMoveBuilder.AppendFormat(@" if ({0}.isPointInPath(x, y)) {{ draw{1}(""{2}""); }};", obfuscatedContextName, obfuscatedCanvasName, "#629BC4");
                                    onMouseMoveBuilder.AppendFormat(@" if (context.isPointInPath(x, y)) {{ {0} = '{1}'; draw{2}(); return; }};", obfuscatedSelectedLayerVariableName, key, target);
                                    if (includeCarriageReturns) { onMouseMoveBuilder.AppendLine(); }
                                }
                            });
                        });
                    });
                }
            }

            // Close function

            drawContentFunctionsBuilder.Append("}");
            if (includeCarriageReturns) { drawContentFunctionsBuilder.AppendLine(); }
        }

        private static string GetObfuscatedBackgroundImageName(string target)
        {
#if DEBUG
            string contextName = string.Format("background{0}", target);
#else
            string contextName = string.Format("bg{0}", GetHashEncodedJavascriptVariableName(target, 4, 12));
#endif
            return contextName;
        }


        private static string GetObfuscatedSelectedLayerVariableName(string target)
        {
#if DEBUG
            string selectedLayerName = string.Format("selected{0}", target);
#else
            string selectedLayerName = string.Format("selected{0}", GetHashEncodedJavascriptVariableName(target, 4, 12));
#endif
            return selectedLayerName;
        }

        private static string GetDrawFunctionNameForLayer(string target, string key)
        {
#if DEBUG
            string contextName = string.Format("draw{0}{1}", target, key);
#else
            string contextName = string.Format("draw{0}{1}", GetHashEncodedJavascriptVariableName(target, 0, 6), GetHashEncodedJavascriptVariableName(key, 0, 6));
#endif
            return contextName;
        }

        private static string GetObfuscatedCanvasName(string target)
        {
#if DEBUG
            string canvasName = string.Format("canvas{0}", target);
#else
            string canvasName = string.Format("a{0}", GetHashEncodedJavascriptVariableName(target, 0, 12));
#endif
            return canvasName;
        }

        private static string GetObfuscatedContextName(string target)
        {
#if DEBUG
            string contextName = string.Format("ctx{0}", target);
#else
            string contextName = string.Format("d{0}", GetHashEncodedJavascriptVariableName(target, 8, 12));
#endif
            return contextName;
        }

        private static string GetResourceDictionaryUri(string url)
        {
            UriBuilder builder = new UriBuilder(HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.Url.Port);

            builder.Path = VirtualPathUtility.ToAbsolute(url);

            return builder.Uri.AbsoluteUri;
        }

        private static string GetHashEncodedJavascriptVariableName(string toHash, int startIndex, int length)
        {
            byte[] hashBytes = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(toHash));

            return BitConverter.ToString(hashBytes).Replace("-", "").Substring(startIndex, length).ToLower();
        }

    }
}