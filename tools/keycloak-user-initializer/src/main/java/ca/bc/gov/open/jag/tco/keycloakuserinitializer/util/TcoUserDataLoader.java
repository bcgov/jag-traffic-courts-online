package ca.bc.gov.open.jag.tco.keycloakuserinitializer.util;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.core.io.Resource;
import org.springframework.core.io.ResourceLoader;
import org.springframework.stereotype.Component;

import com.opencsv.bean.CsvToBeanBuilder;

import ca.bc.gov.open.jag.tco.keycloakuserinitializer.model.TcoUser;

@Component
public class TcoUserDataLoader {
	
	@Autowired
	private ResourceLoader resourceLoader;
	
	public List<TcoUser> getTcoUsers() throws IllegalArgumentException{

		List<TcoUser> users = new ArrayList<TcoUser>();
		try (
			InputStream is = getFileAsIOStream("data/tco-user-list.csv");
			BufferedReader reader = new BufferedReader(new InputStreamReader(is))) {
			users.addAll(new CsvToBeanBuilder<TcoUser>(reader)
					.withType(TcoUser.class)
					.withSkipLines(1)
					.build()
					.parse());
		} catch (IOException e) {
			e.printStackTrace();
		}
		return users;
	}
	
	private InputStream getFileAsIOStream(final String fileName) {
        InputStream ioStream = this.getClass()
                .getClassLoader()
                .getResourceAsStream(fileName);
        
        if (ioStream == null) {
            throw new IllegalArgumentException(fileName + " is not found");
        }
        return ioStream;
    }

	// Alternative way of loading the file from the classpath
	public InputStream loadFile() throws IOException {
		Resource resource = resourceLoader.getResource("classpath:data/tco-user-list.csv");
		InputStream inputStream = resource.getInputStream();
		return inputStream;
	}
}
