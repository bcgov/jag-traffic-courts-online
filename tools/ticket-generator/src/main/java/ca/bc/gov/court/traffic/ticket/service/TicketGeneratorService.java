package ca.bc.gov.court.traffic.ticket.service;

import java.awt.Color;
import java.awt.FontFormatException;
import java.awt.FontMetrics;
import java.awt.Graphics;
import java.awt.Image;
import java.awt.Point;
import java.awt.image.BufferedImage;
import java.io.IOException;
import java.net.URISyntaxException;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import ca.bc.gov.court.traffic.ticket.model.Style;
import ca.bc.gov.court.traffic.ticket.model.ViolationTicket;
import ca.bc.gov.court.traffic.ticket.util.RandomUtil;
import ca.bc.gov.court.traffic.ticket.util.ResourceLoader;

@Service
public class TicketGeneratorService {

	private static final Logger logger = LogManager.getLogger(TicketGeneratorService.class);

	@Autowired
	private BufferedImage blankTicket;

	@Autowired
	private ResourceLoader resourceLoader;

	public BufferedImage createTicket(Integer style, Integer numCounts) throws Exception {
		return createTicket(null, style, numCounts);
	}

	public BufferedImage createTicket(ViolationTicket violationTicket, Integer style, Integer numCounts) throws Exception {
		logger.debug("Generating random ticket");

		if (violationTicket == null) {
			violationTicket = RandomUtil.randomTicket(numCounts);
		}
		int writingStyle = (style == null) ? Integer.valueOf(RandomUtil.randomInt(1, 8)) : style.intValue();

		// Copy blankTicket to new bufferedImage
		BufferedImage ticketImage = new BufferedImage(blankTicket.getWidth(), blankTicket.getHeight(), BufferedImage.TYPE_INT_ARGB);
		Graphics g = ticketImage.getGraphics();
		g.drawImage(blankTicket, 0, 0, null);

		g.setColor(Color.BLACK);
		drawViolationTicketNumber(violationTicket, g);

		g.setColor(Color.BLUE);
		drawField(g, writingStyle, violationTicket.getSurname(), 200, 888, 2691, 543, 584, 690);
		drawField(g, writingStyle, violationTicket.getGivenName(), 200, 1235, 2294, 695, 734, 840);
		drawField(g, writingStyle, violationTicket.getIsYoungPerson(), 2338, 2338, 2411, 730, 730, 805);
		drawField(g, writingStyle, violationTicket.getDriversLicenceProvince(), 200, 200, 535, 885, 885, 992);
		drawField(g, writingStyle, violationTicket.getDriversLicenceNumber(), 534, 534, 1434, 885, 885, 992);
		drawField(g, writingStyle, violationTicket.getDriversLicenceCreated(), 1437, 1437, 1690, 885, 885, 992);
		drawField(g, writingStyle, violationTicket.getDriversLicenceExpiry(), 1694, 1694, 1911, 885, 885, 992);
		drawField(g, writingStyle, violationTicket.getBirthdateYYYY(), 2135, 2135, 2339, 885, 885, 992);
		drawField(g, writingStyle, violationTicket.getBirthdateMM(), 2342, 2342, 2522, 885, 885, 992);
		drawField(g, writingStyle, violationTicket.getBirthdateDD(), 2526, 2526, 2691, 885, 885, 992);
		drawField(g, writingStyle, violationTicket.getAddress(), 200, 408, 2298, 993, 1035, 1143);
		drawField(g, writingStyle, violationTicket.getIsChangeOfAddress(), 2330, 2330, 2410, 1025, 1025, 1125);
		drawField(g, writingStyle, violationTicket.getCity(), 200, 311, 1547, 1145, 1187, 1293);
		drawField(g, writingStyle, violationTicket.getProvince(), 1548, 1839, 1995, 1145, 1187, 1293);
		drawField(g, writingStyle, violationTicket.getPostalCode(), 1995, 2396, 2694, 1145, 1187, 1293);
		drawField(g, writingStyle, violationTicket.getNamedIsDriver(), 762, 762, 803, 1392, 1392, 1435);
		drawField(g, writingStyle, violationTicket.getNamedIsCyclist(), 1212, 1212, 1253, 1392, 1392, 1435);
		drawField(g, writingStyle, violationTicket.getNamedIsOwner(), 1659, 1659, 1703, 1392, 1392, 1435);
		drawField(g, writingStyle, violationTicket.getNamedIsPedestrain(), 762, 762, 803, 1468, 1468, 1510);
		drawField(g, writingStyle, violationTicket.getNamedIsPassenger(), 1210, 1210, 1253, 1468, 1468, 1510);
		drawField(g, writingStyle, violationTicket.getNamedIsOther(), 1660, 1660, 1703, 1468, 1468, 1510);
		drawField(g, writingStyle, violationTicket.getNamedIsOtherDescription(), 1897, 1897, 2700, 1445, 1445, 1507);
		drawField(g, writingStyle, violationTicket.getViolationDateYYYY(), 558, 558, 929, 1598, 1598, 1688);
		drawField(g, writingStyle, violationTicket.getViolationDateMM(), 929, 929, 1170, 1598, 1598, 1688);
		drawField(g, writingStyle, violationTicket.getViolationDateDD(), 1170, 1170, 1436, 1598, 1598, 1688);
		drawField(g, writingStyle, violationTicket.getViolationTimeHH(), 1857, 1857, 2100, 1562, 1562, 1685);
		drawField(g, writingStyle, violationTicket.getViolationTimeMM(), 2100, 2100, 2346, 1562, 1562, 1685);
		drawField(g, writingStyle, violationTicket.getViolationOnHighway(), 280, 280, 2692, 1686, 1686, 1833);
		drawField(g, writingStyle, violationTicket.getViolationNearPlace(), 460, 460, 2040, 1873, 1873, 1965);
		drawField(g, writingStyle, violationTicket.getOffenseIsMVA(), 200, 200, 242, 2104, 2104, 2146);
		drawField(g, writingStyle, violationTicket.getOffenseIsWLA(), 1100, 1100, 1142, 2104, 2104, 2146);
		drawField(g, writingStyle, violationTicket.getOffenseIsLCA(), 1700, 1700, 1742, 2104, 2104, 2146);
		drawField(g, writingStyle, violationTicket.getOffenseIsMCA(), 200, 200, 242, 2180, 2180, 2222);
		drawField(g, writingStyle, violationTicket.getOffenseIsFAA(), 1100, 1100, 1142, 2180, 2180, 2222);
		drawField(g, writingStyle, violationTicket.getOffenseIsTCR(), 1700, 1742, 1765, 2180, 2180, 2222);
		drawField(g, writingStyle, violationTicket.getOffenseIsCTA(), 200, 200, 242, 2254, 2254, 2296);
		drawField(g, writingStyle, violationTicket.getOffenseIsOther(), 1100, 1100, 1142, 2254, 2254, 2296);
		drawField(g, writingStyle, violationTicket.getOffenseIsOtherDescription(), 1546, 1546, 2700, 2237, 2237, 2308);
		drawField(g, writingStyle, violationTicket.getCount1Description(), 278, 278, 1474, 2454, 2454, 2753);
		drawField(g, writingStyle, violationTicket.getCount1ActReg(), 1475, 1475, 1775, 2454, 2454, 2603);
		drawField(g, writingStyle, violationTicket.getCount1IsACT(), 1525, 1525, 1567, 2625, 2625, 2668);
		drawField(g, writingStyle, violationTicket.getCount1IsREGS(), 1525, 1525, 1567, 2684, 2684, 2727);
		drawField(g, writingStyle, violationTicket.getCount1Section(), 1775, 1775, 2373, 2455, 2455, 2755);
		drawField(g, writingStyle, violationTicket.getCount1TicketAmount(), 2437, 2437, 2700, 2456, 2456, 2670);
		drawField(g, writingStyle, violationTicket.getCount2Description(), 278, 278, 1474, 2755, 2755, 3055);
		drawField(g, writingStyle, violationTicket.getCount2ActReg(), 1475, 1475, 1775, 2755, 2755, 2902);
		drawField(g, writingStyle, violationTicket.getCount2IsACT(), 1513, 1513, 1556, 2927, 2927, 2969);
		drawField(g, writingStyle, violationTicket.getCount2IsREGS(), 1513, 1513, 1556, 2986, 2986, 3028);
		drawField(g, writingStyle, violationTicket.getCount2Section(), 1775, 1775, 2373, 2755, 2755, 3055);
		drawField(g, writingStyle, violationTicket.getCount2TicketAmount(), 2437, 2437, 2700, 2755, 2755, 2969);
		drawField(g, writingStyle, violationTicket.getCount3Description(), 278, 278, 1474, 3055, 3055, 3356);
		drawField(g, writingStyle, violationTicket.getCount3ActReg(), 1475, 1475, 1775, 3055, 3055, 3203);
		drawField(g, writingStyle, violationTicket.getCount3IsACT(), 1513, 1513, 1556, 3227, 3227, 3270);
		drawField(g, writingStyle, violationTicket.getCount3IsREGS(), 1513, 1513, 1556, 3287, 3287, 3329);
		drawField(g, writingStyle, violationTicket.getCount3Section(), 1775, 1775, 2373, 3055, 3055, 3355);
		drawField(g, writingStyle, violationTicket.getCount3TicketAmount(), 2437, 2437, 2700, 3057, 3057, 3272);
		drawField(g, writingStyle, violationTicket.getVehicleLicensePlateProvince(), 764, 764, 1101, 3547, 3547, 3633);
		drawField(g, writingStyle, violationTicket.getVehicleLicensePlateNumber(), 1102, 1102, 1701, 3547, 3547, 3633);
		drawField(g, writingStyle, violationTicket.getVehicleNscPuj(), 1701, 1701, 1952, 3547, 3547, 3633);
		drawField(g, writingStyle, violationTicket.getVehicleNscNumber(), 1952, 1952, 2500, 3547, 3547, 3633);
		drawField(g, writingStyle, violationTicket.getVehicleRegisteredOwnerName(), 203, 768, 2450, 3633, 3674, 3758);
		drawField(g, writingStyle, violationTicket.getVehicleMake(), 204, 513, 802, 3759, 3802, 3882);
		drawField(g, writingStyle, violationTicket.getVehicleType(), 802, 920, 1248, 3759, 3802, 3883);
		drawField(g, writingStyle, violationTicket.getVehicleColour(), 1248, 1440, 1696, 3759, 3802, 3882);
		drawField(g, writingStyle, violationTicket.getVehicleYear(), 1696, 1820, 2113, 3759, 3802, 3882);
		drawField(g, writingStyle, violationTicket.getNoticeOfDisputeAddress(), 203, 928, 2450, 3930, 3982, 4101);
		drawField(g, writingStyle, violationTicket.getHearingLocation(), 203, 1030, 1556, 4161, 4199, 4285);
		drawField(g, writingStyle, violationTicket.getEnforcementOfficerNumber(), 203, 562, 802, 4286, 4323, 4410);
		drawField(g, writingStyle, violationTicket.getDetachmentLocation(), 803, 1687, 2450, 4287, 4328, 4410);
		drawField(g, writingStyle, violationTicket.getDateOfServiceYYYY(), 1780, 1780, 2006, 4195, 4195, 4286);
		drawField(g, writingStyle, violationTicket.getDateOfServiceMM(), 2006, 2006, 2229, 4195, 4195, 4286);
		drawField(g, writingStyle, violationTicket.getDateOfServiceDD(), 2229, 2229, 2450, 4195, 4195, 4286);

		// text box used during development to determine where to place the above fields
//		BufferedImage box = ResourceLoader.getBox();
//		g.drawImage(box, 1505, 3224, null);
//		g.drawImage(box, 1505, 3284, null);
//		g.drawImage(box, 2700 - 75, 2308 - 75, null);

		return resizeImage(ticketImage, blankTicket.getWidth() / 2, blankTicket.getHeight() / 2);
	}

	BufferedImage resizeImage(BufferedImage originalImage, int targetWidth, int targetHeight) throws IOException {
	    Image resultingImage = originalImage.getScaledInstance(targetWidth, targetHeight, Image.SCALE_DEFAULT);
	    BufferedImage outputImage = new BufferedImage(targetWidth, targetHeight, BufferedImage.TYPE_INT_RGB);
	    outputImage.getGraphics().drawImage(resultingImage, 0, 0, null);
	    return outputImage;
	}

	private void drawViolationTicketNumber(ViolationTicket violationTicket, Graphics g) throws Exception {
		String text = violationTicket.getViolationTicketNumber();
		Style style = resourceLoader.getStyle(99, 210);
		FontMetrics metrics = g.getFontMetrics(style.getFont());
		int stringWidth = metrics.stringWidth(text);
		g.setFont(style.getFont());
		g.drawString(text, 2660 - stringWidth, 506);
	}

	private void drawField(Graphics g, int writingStyle, String text, int x1, int x2, int x3, int y1, int y2, int y3) throws FontFormatException, IOException, URISyntaxException {
		if (text == null)
			return;

		int fieldWidth = x3 - x1;
		int fieldHeight = y3 - y1;
		double fontSize = RandomUtil.randomDouble(95, 135);
		Style style = resourceLoader.getStyle(writingStyle, fontSize);
		g.setFont(style.getFont());
		FontMetrics metrics = g.getFontMetrics(style.getFont());
		String[] lines = (metrics.stringWidth(text) > fieldWidth) ? splitPhrase(text) : new String[] { text };

		// find longest string
		String longestText = "";
		for (int i = 0; i < lines.length; i++) {
			if (lines[i].length() > longestText.length())
				longestText = lines[i];
		}

		// if too large, resize to the largest font size that will fit the target width
		while (metrics.stringWidth(longestText) > fieldWidth) {
			fontSize = fontSize - 10.0;
			style = resourceLoader.getStyle(writingStyle, fontSize);
			g.setFont(style.getFont());
			metrics = g.getFontMetrics(style.getFont());
		}

		int rowHeight = fieldHeight / lines.length;
		for (int i = 0; i < lines.length; i++) {
			Point location = RandomUtil.getRandomLocation(style, metrics.stringWidth(lines[0]), metrics.getAscent(),
					x1, x2, x3,
					y1 + (rowHeight*i),
					y2 + (rowHeight*i),
					y1 + (rowHeight*i) + rowHeight);
			g.drawString(lines[i], location.x, location.y);
		}
	}

	private String[] splitPhrase(String text) {
		if (!text.contains(" ")) { // cannot split - probably a single word.
			return new String[] { text };
		}
		try {
		    // split text around middle, find closes space and split on that.
		    int len = text.length();
		    String a = text.substring(0, len/2);
		    String b = text.substring(len/2, len);
		    String c = b.substring(0, b.indexOf(' '));
		    String d = b.substring(b.indexOf(' ')+1, b.length());
		    String line1 = a + c;
		    String line2 = d;
		    return new String[] { line1, line2 };
		} catch (Exception e) {
			// Unable to split phrase. Return original, but log error
			logger.error("Unable to split phrase, '" + text + "'", e);
		    return new String[] { text };
		}
	}

}
