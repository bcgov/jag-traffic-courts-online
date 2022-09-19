package ca.bc.gov.open.jag.tco.oracledataapi.config;

import java.time.Duration;
import java.util.concurrent.TimeUnit;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.http.HttpHeaders;
import org.springframework.http.client.reactive.ReactorClientHttpConnector;
import org.springframework.web.reactive.function.client.ExchangeFilterFunction;
import org.springframework.web.reactive.function.client.ExchangeStrategies;
import org.springframework.web.reactive.function.client.WebClient;

import ca.bc.gov.open.jag.tco.oracledataapi.error.OracleDataApiException;
import io.netty.channel.ChannelOption;
import io.netty.handler.timeout.ReadTimeoutHandler;
import io.netty.handler.timeout.WriteTimeoutHandler;
import reactor.netty.http.client.HttpClient;
import reactor.util.retry.Retry;

@Configuration
public class WebClientConfiguration {
	
	private final Logger logger = LoggerFactory.getLogger(WebClientConfiguration.class);
	
	@Autowired
    ConfigProperties properties;
	
	@Bean
    public WebClient ordsWebClient() {
        final var httpClient = HttpClient
                .create()
                .option(ChannelOption.CONNECT_TIMEOUT_MILLIS, properties.getOrdsRestApiTimeout())
                .doOnConnected(connection -> {
                    connection.addHandlerLast(new ReadTimeoutHandler(properties.getOrdsRestApiTimeout(), TimeUnit.MILLISECONDS));
                    connection.addHandlerLast(new WriteTimeoutHandler(properties.getOrdsRestApiTimeout(), TimeUnit.MILLISECONDS));
                });
        
        final int size = 16 * 1024 * 1024;
        final ExchangeStrategies strategies = ExchangeStrategies.builder()
            .codecs(codecs -> codecs.defaultCodecs().maxInMemorySize(size))
            .build();

        return WebClient.builder()
                .baseUrl(properties.getOrdsRestApiUrl())
                .clientConnector(new ReactorClientHttpConnector(httpClient))
                .defaultHeader(HttpHeaders.CONTENT_TYPE, "application/json")
                .exchangeStrategies(strategies)
                .filter(retryFilter())
                .build();
    }
	
	
	/**
	 * Connection retry configuration
	 * 
	 * @return {@link ExchangeFilterFunction}
	 */
	private ExchangeFilterFunction retryFilter() {
		return (request, next) ->
			next.exchange(request)
	        	.retryWhen(
		            Retry.fixedDelay(properties.getOrdsRestApiRetryCount(), Duration.ofSeconds(properties.getOrdsRestApiRetryDelay()))
		            .doAfterRetry(retrySignal -> {
	                    logger.info("Retried " + retrySignal.totalRetries());
	                })
		            .onRetryExhaustedThrow((retryBackoffSpec, retrySignal) ->
	                new OracleDataApiException(
	                    "ORDS API failed to respond, after max attempts of: "
	                    + retrySignal.totalRetries())));
	}
}
