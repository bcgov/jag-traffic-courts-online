package ca.bc.gov.open.jag.tco.oracledataapi.config;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.SerializationFeature;
import com.fasterxml.jackson.databind.module.SimpleModule;
import com.fasterxml.jackson.datatype.jsr310.JavaTimeModule;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.Primary;

import java.util.Date;

/**
 * Jackson configuration for consistent date/time formatting across all endpoints.
 * Uses timezone-neutral approach to preserve database values without any conversion.
 * This configuration overrides any @JsonFormat annotations from the models.
 */
@Configuration
public class JacksonConfig {

    @Bean
    @Primary
    public ObjectMapper objectMapper() {
        ObjectMapper mapper = new ObjectMapper();
        
        // Disable writing dates as timestamps
        mapper.disable(SerializationFeature.WRITE_DATES_AS_TIMESTAMPS);
        
        // DO NOT set timezone - this keeps it timezone-neutral
        
        // Register JavaTimeModule for JSR-310 support
        mapper.registerModule(new JavaTimeModule());
        
        // Create a custom module to handle Date serialization globally
        SimpleModule dateModule = new SimpleModule("DateModule");
        dateModule.addSerializer(Date.class, new GlobalDateSerializer());
        mapper.registerModule(dateModule);
        
        return mapper;
    }
}