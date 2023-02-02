import { Pipe, PipeTransform } from "@angular/core";
import { DatePipe } from "@angular/common";
import { isString } from "lodash";

@Pipe({ name: "date" })
export class CustomDatePipe extends DatePipe implements PipeTransform {
  transform(value: Date | string | number, format?: string, timezone?: string, locale?: string): string | null;
  transform(value: null | undefined, format?: string, timezone?: string, locale?: string): null;
  transform(value: Date | string | number | null | undefined, format?: string, timezone?: string, locale?: string): string | null {
    let arr = format?.split("ddd"); // adding "ddd" to default DatePipe
    if (value) {
      if (isString(value) && value.indexOf('T') > -1) {
        value = value.split("T")[0];
      }
      let outputArr = arr.map(i => super.transform(value, i));
      let seperator = "";
      if (arr.length > 1) {
        let day = super.transform(value, "d");
        let suffix = "th";
        if (day === "1" || day === "21" || day === "31") {
          suffix = "st"
        } else if (day === "2" || day === "22") {
          suffix = "nd";
        } else if (day === "3" || day === "23") {
          suffix = "rd";
        }
        seperator = day + suffix;
      }
      return outputArr.join(seperator);
    } else {
      return null;
    }
  }
}