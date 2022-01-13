/// <summary>
/// A model representation of the extracted OCR results.
/// </summary>
public class ViolationTicket
{
    public ViolationTicket() { }

    /// <summary>
    /// Confidence of correctly extracting the document.
    /// </summary>
    public float confidence { get; set; }

    /// <summary>
    /// An enumeration of all fields in this Violation Ticket.
    /// </summary>
    public List<Field> fields { get; set; } = new List<Field>();

    public class Field
    {
        public String? name { get; set; }
        public String? value { get; set; }
        public float? confidence { get; set; }
        public String? type { get; set; }
        public List<BoundingBox> boundingBoxes { get; set; } = new List<BoundingBox>();
    }

    public class BoundingBox
    {
        public List<Point> points { get; set; } = new List<Point>();
    }

    public class Point
    {
        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public float? x { get; set; }
        public float? y { get; set; }
    }

}