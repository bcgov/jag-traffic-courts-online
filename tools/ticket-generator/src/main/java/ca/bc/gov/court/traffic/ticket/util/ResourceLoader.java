package ca.bc.gov.court.traffic.ticket.util;

import java.awt.Font;
import java.awt.FontFormatException;
import java.io.IOException;
import java.net.URISyntaxException;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import ca.bc.gov.court.traffic.ticket.model.Style;

@Component
public class ResourceLoader {
	
	@Autowired
	private Font caveatVariableFont;

	@Autowired
	private Font gloriaHallelujahFont;

	@Autowired
	private Font homemadeAppleFont;

	@Autowired
	private Font indieFlowerFont;

	@Autowired
	private Font nanumPenScriptFont;

	@Autowired
	private Font rockSaltFont;

	@Autowired
	private Font shadowsIntoLightFont;

	@Autowired
	private Font vujahdayScriptFont;

	public Style getStyle(int index, double size) throws FontFormatException, IOException, URISyntaxException {
		Style style = null;
		switch (index) {
		case 1:
			style = new Style(caveatVariableFont, size, 0, 1.5, 1.0);
			break;
		case 2:
			style = new Style(gloriaHallelujahFont, size, 0, 1.7, 0.78);
			break;
		case 3:
			style = new Style(homemadeAppleFont, size, -2, 1.4, 0.68);
			break;
		case 4:
			style = new Style(indieFlowerFont, size, 0, 1.6, 0.93);
			break;
		case 5:
			style = new Style(nanumPenScriptFont, size, 0, 1.6, 1.15);
			break;
		case 6:
			style = new Style(rockSaltFont, size, 0, 1.6, 0.56);
			break;
		case 7:
			style = new Style(shadowsIntoLightFont, size, 0, 1.7, 1.0);
			break;
		case 8:
			style = new Style(vujahdayScriptFont, size, 0, 1.5, 0.75);
			break;
		default:
			style = new Style(new Font("Serif", Font.BOLD, 10), size, 0, 1.5, 0.75);
			break;
		}

		return style;
	}

}
