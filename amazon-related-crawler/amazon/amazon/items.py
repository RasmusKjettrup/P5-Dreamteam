# -*- coding: utf-8 -*-

# Define here the models for your scraped items
#
# See documentation in:
# http://doc.scrapy.org/en/latest/topics/items.html

import scrapy


class Book(scrapy.Item):
    name = scrapy.Field()
    price = scrapy.Field()
    related = scrapy.Field()