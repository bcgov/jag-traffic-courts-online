package ca.bc.gov.court.traffic.ticket.model;

import java.awt.Font;

public class Style {

	private Font font;
	private int heightOffset;
	private double heightScale;
	private double fontScale;
	
	public Style(Font font, float size) {
		this.font = font.deriveFont(size);
		this.heightOffset = 0;
		this.heightScale = 1.0;
	}
	
	public Style(Font font, double size, int heightOffset, double heightScale, double fontScale) {
		this.font = font.deriveFont((float) (size * fontScale));
		this.heightOffset = heightOffset;
		this.heightScale = heightScale;
	}
	
	public Font getFont() {
		return font;
	}
	
	public int getHeightOffset() {
		return heightOffset;
	}
	
	public double getHeightScale() {
		return heightScale;
	}
	
	public double getFontScale() {
		return fontScale;
	}
	
}
