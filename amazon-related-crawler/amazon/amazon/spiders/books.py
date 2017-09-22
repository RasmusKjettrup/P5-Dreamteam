# -*- coding: utf-8 -*-
import scrapy
import re
import time
from collections import deque


class BooksSpider(scrapy.Spider):
    name = 'books'
    allowed_domains = ['amazon.com']
    start_urls = ['https://www.amazon.com/gp/product/1787300080/']

    def __init__(self):
        global relation_list 
        global relation_list_length
        global queue
        queue = deque()
        relation_list = []
        relation_list_length = 0

    def parse(page,response):
        global relation_list 
        global relation_list_length
        global queue
        booktitle = response.xpath('//*[@id="productTitle"]/text()').extract_first().strip()
        print(booktitle)

        regex = re.compile('(Paperback|Hardcover)')
        formats = response.xpath('//div[@id="tmmSwatches"]').extract_first()
        result = regex.search(formats)

        if not result:
            print('            Not a book!')
            return

        links = response.css('li.a-carousel-card > div:first-child > a:first-child::attr(href)').extract()
        names = response.css('li.a-carousel-card > div:first-child > a:first-child > div:nth-child(2)::text').extract()

        for i in range(len(names)):
            names[i] = names[i].strip()

        nameCount = 0
        for link in links:
            queue.append(response.urljoin(link))
            relation_list.append((booktitle, names[nameCount]))
            print(relation_list[relation_list_length])
            relation_list_length += 1
            nameCount += 1

        while (len(queue) != 0):
            yield scrapy.Request(queue.popleft(), callback=page.parse)