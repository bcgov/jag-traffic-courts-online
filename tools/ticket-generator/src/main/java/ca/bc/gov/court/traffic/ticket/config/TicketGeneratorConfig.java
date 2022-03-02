package ca.bc.gov.court.traffic.ticket.config;

import java.awt.Font;
import java.awt.FontFormatException;
import java.awt.image.BufferedImage;
import java.io.IOException;
import java.net.URL;

import javax.imageio.ImageIO;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.core.io.Resource;

@Configuration
public class TicketGeneratorConfig {

	@Value("classpath:img/blank-traffic-ticket-aligned-cleaned.png")
	private Resource blankImage;

	@Value("classpath:font/Caveat-VariableFont_wght.ttf")
	private Resource caveatVariableFont;

	@Value("classpath:font/GloriaHallelujah-Regular.ttf")
	private Resource gloriaHallelujahFont;

	@Value("classpath:font/HomemadeApple-Regular.ttf")
	private Resource homemadeAppleFont;

	@Value("classpath:font/IndieFlower-Regular.ttf")
	private Resource indieFlowerFont;

	@Value("classpath:font/NanumPenScript-Regular.ttf")
	private Resource nanumPenScriptFont;

	@Value("classpath:font/RockSalt-Regular.ttf")
	private Resource rockSaltFont;

	@Value("classpath:font/ShadowsIntoLight-Regular.ttf")
	private Resource shadowsIntoLightFont;

	@Value("classpath:font/VujahdayScript-Regular.ttf")
	private Resource vujahdayScriptFont;

	@Bean
	public BufferedImage blankTicket() throws IOException {
		URL ticketUrl = blankImage.getURL();
		BufferedImage ticket = ImageIO.read(ticketUrl);
		return ticket;
	}

	@Bean
	public Font caveatVariableFont() throws FontFormatException, IOException {
		return Font.createFont(Font.PLAIN, caveatVariableFont.getInputStream());
	}

	@Bean
	public Font gloriaHallelujahFont() throws FontFormatException, IOException {
		return Font.createFont(Font.PLAIN, gloriaHallelujahFont.getInputStream());
	}

	@Bean
	public Font homemadeAppleFont() throws FontFormatException, IOException {
		return Font.createFont(Font.PLAIN, homemadeAppleFont.getInputStream());
	}

	@Bean
	public Font indieFlowerFont() throws FontFormatException, IOException {
		return Font.createFont(Font.PLAIN, indieFlowerFont.getInputStream());
	}

	@Bean
	public Font nanumPenScriptFont() throws FontFormatException, IOException {
		return Font.createFont(Font.PLAIN, nanumPenScriptFont.getInputStream());
	}

	@Bean
	public Font rockSaltFont() throws FontFormatException, IOException {
		return Font.createFont(Font.PLAIN, rockSaltFont.getInputStream());
	}

	@Bean
	public Font shadowsIntoLightFont() throws FontFormatException, IOException {
		return Font.createFont(Font.PLAIN, shadowsIntoLightFont.getInputStream());
	}

	@Bean
	public Font vujahdayScriptFont() throws FontFormatException, IOException {
		return Font.createFont(Font.PLAIN, vujahdayScriptFont.getInputStream());
	}

}
