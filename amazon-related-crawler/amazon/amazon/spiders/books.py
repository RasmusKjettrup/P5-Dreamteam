# -*- coding: utf-8 -*-
import scrapy


class BooksSpider(scrapy.Spider):
    name = 'books'
    allowed_domains = ['amazon.com']
    start_urls = ['https://www.amazon.com/gp/product/1787300080/']

    def parse(self, response):
        booktitle = response.xpath('//*[@id="productTitle"]/text()').extract_first()
        print(booktitle)

        links = response.css('li.a-carousel-card > div:first-child > a:first-child::attr(href)').extract()
        #print(links)
        for link in links:
            next_page = response.urljoin(link)
            yield scrapy.Request(next_page, callback=self.parse)
