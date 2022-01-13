/// <summary>
/// A model representation of the extracted OCR results.
/// </summary>
public class ViolationTicket
{
    public ViolationTicket() { }

    /// <summary>
    /// Confidence of correctly extracting the document.
    /// </summary>
    public float Confidence { get; set; }

    /// <summary>
    /// An enumeration of all fields in this Violation Ticket.
    /// </summary>
    public List<Field> Fields { get; set; } = new List<Field>();

    public class Field
    {
        public String? Name { get; set; }
        public String? Value { get; set; }
        public float? Confidence { get; set; }
        public String? Type { get; set; }
        public List<BoundingBox> BoundingBoxes { get; set; } = new List<BoundingBox>();
    }

    public class BoundingBox
    {
        public List<Point> Points { get; set; } = new List<Point>();
    }

    public class Point
    {
        public Point(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public float X { get; set; }
        public float Y { get; set; }
    }

}
