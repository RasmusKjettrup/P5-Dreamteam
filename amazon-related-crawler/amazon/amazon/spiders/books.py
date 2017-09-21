# -*- coding: utf-8 -*-
import scrapy


class BooksSpider(scrapy.Spider):
    name = 'books'
    allowed_domains = ['amazon.com']
    start_urls = ['https://www.amazon.co.uk/gp/product/1787300080/']

    def parse(self, response):
        booktitle = response.xpath('//*[@id="productTitle"]/text()').extract_first()
        print(booktitle)

        next_page = response.css("li.a-carousel-card a::attr(href)").extract_first()
        if next_page is not None:
            next_page = response.urljoin(next_page)
            yield scrapy.Request(next_page, callback=self.parse)
