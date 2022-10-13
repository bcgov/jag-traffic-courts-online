using TrafficCourts.Common.Features.Mail;

namespace TrafficCourts.Common;

public interface IEmailTemplate<T>
{
    /// <summary>
    /// Create email message based on the data
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="EmailTemplateNameNotFoundException"></exception>
    EmailMessage Create(T data);
}
