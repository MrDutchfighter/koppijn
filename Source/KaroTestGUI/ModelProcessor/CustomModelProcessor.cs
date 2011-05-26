using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace CustomModelProcessor
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
    [ContentProcessor(DisplayName = "CustomModelProcessor.CustomModelProcessor")]
    public class CustomModelProcessor : ModelProcessor
    {
        float minX = float.MaxValue;
        float maxX = float.MinValue;

        float minY = float.MaxValue;
        float maxY = float.MinValue;

        float minZ = float.MaxValue;
        float maxZ = float.MinValue;

        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            this.RotationX = -90;

            ModelContent content = base.Process(input, context);

            if (input is MeshContent)
            {
                parseChildren((MeshContent)input);
            }
            else
            {
                parseChildren(input.Children);
            }

            //VertexPositionColor[] points = new VertexPositionColor[8];
            //front

            //points[0] = new VertexPositionColor(new Vector3(minX, minY, minZ), Color.Gold);
            //points[1] = new VertexPositionColor(new Vector3(maxX, minY, minZ), Color.Gold);
            //points[2] = new VertexPositionColor(new Vector3(minX, maxY, minZ), Color.Gold);
            //points[3] = new VertexPositionColor(new Vector3(maxX, maxY, minZ), Color.Gold);
            
            ////back
            //points[4] = new VertexPositionColor(new Vector3(minX, minY, maxZ), Color.Gold);
            //points[5] = new VertexPositionColor(new Vector3(maxX, minY, maxZ), Color.Gold);
            //points[6] = new VertexPositionColor(new Vector3(minX, maxY, maxZ), Color.Gold);
            //points[7] = new VertexPositionColor(new Vector3(maxX, maxY, maxZ), Color.Gold);
            
            content.Tag = new BoundingBox(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
            //content.Tag = points;
            return content;
        }

        private void parseChildren(MeshContent meshContent)
        {
            foreach (Vector3 vector in meshContent.Positions)
            {
                if (vector.X < minX)
                    minX = vector.X;

                if (vector.Y < minY)
                    minY = vector.Y;

                if (vector.Z < minZ)
                    minZ = vector.Z;

                if (vector.X > maxX)
                    maxX = vector.X;

                if (vector.Y > maxY)
                    maxY = vector.Y;

                if (vector.Z > maxZ)
                    maxZ = vector.Z;
            }
        }

        private void parseChildren(NodeContentCollection nodeContentCollection)
        {
            foreach (NodeContent nodeContent in nodeContentCollection)
            {
                if (nodeContent is MeshContent)
                {
                    MeshContent meshContent = (MeshContent)nodeContent;

                    foreach (Vector3 vector in meshContent.Positions)
                    {
                        if (vector.X < minX)
                            minX = vector.X;

                        if (vector.Y < minY)
                            minY = vector.Y;

                        if (vector.Z < minZ)
                            minZ = vector.Z;

                        if (vector.X > maxX)
                            maxX = vector.X;

                        if (vector.Y > maxY)
                            maxY = vector.Y;

                        if (vector.Z > maxZ)
                            maxZ = vector.Z;
                    }
                }
                else
                {
                    parseChildren(nodeContent.Children);
                }
            }
        }
    }
}