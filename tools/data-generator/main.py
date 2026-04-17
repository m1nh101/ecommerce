#!/usr/bin/env python3

import sys
import argparse
import psycopg2
from psycopg2.extras import execute_values

from faker import Faker
from faker_ecommerce import EcommerceProvider

fake = Faker()
fake.add_provider(EcommerceProvider)

SUPPORT_TABLES = ['products']

def verify_table(table_name: str) -> bool:
    return table_name in SUPPORT_TABLES

def verify_connection_string(connection_string: str) -> bool:
    # Placeholder for actual connection string validation logic
    try:
        conn = psycopg2.connect(connection_string)
        conn.close()
    except psycopg2.OperationalError as e:
        print(f"Database connection error: {e}")
        return False

    return True

def verify(table_name: str, connection_string: str) -> bool:
    if not verify_table(table_name):
        print(f"Error: Unsupported table '{table_name}'. Supported tables are: {', '.join(SUPPORT_TABLES)}")
        return False
    if not verify_connection_string(connection_string):
        print("Error: Invalid database connection string.")
        return False
    return True

def generate_products_data(number: int) -> list[(str, str, str, int, int, str)]:
    src = []

    for _ in range(number):
        row = (fake.uuid4(), fake.product_name(), fake.product_description(), fake.random_int(0, 1_000_000), fake.random_int(0, 1000), fake.product_category())
        src.append(row)
    
    return src

def insert_products_data(connection_string: str, data: list[(str, str, int, int, str)]):
    conn = None
    cursor = None
    try:
        conn = psycopg2.connect(connection_string)
        insert_query = """
            INSERT INTO products (id, name, description, price, currency, stock_quantity, category, is_active, created_at, updated_at)
            VALUES %s"""
        template = "(%s, %s, %s, %s, 'VND', %s, %s, true, now(), now())"

        with conn:
            with conn.cursor() as cursor:
                execute_values(cursor, insert_query, data, template=template)
    except psycopg2.Error as e:
        print(f"Database error: {e}")
    finally:
        if cursor:
            cursor.close()
        if conn:
            conn.close()

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Data generator for ecommerce")
    parser.add_argument('table', help='Name of the table to generate data for')
    parser.add_argument('number', type=int, help='Number of records to generate')
    parser.add_argument('db_connection', help='Database connection string')
    args = parser.parse_args()

    if not verify(args.table, args.db_connection):
        sys.exit(1)

    insert_products_data(args.db_connection, generate_products_data(args.number))