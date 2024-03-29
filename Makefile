.PHONY: setup
setup:
	docker-compose build

.PHONY: build
build:
	docker-compose build housing-register-api

.PHONY: serve
serve:
	docker-compose build housing-register-api && docker-compose up housing-register-api

serve-from-aws:
	docker-compose build housing-register-api-remote && docker-compose up housing-register-api-remote

.PHONY: shell
shell:
	docker-compose run housing-register-api bash

.PHONY: test
test:
	docker-compose up -d dynamodb-database
	docker-compose build housing-register-api-test && docker-compose up housing-register-api-test

.PHONY: lint
lint:
	-dotnet tool install -g dotnet-format
	dotnet tool update -g dotnet-format
	dotnet format

.PHONY: restart-db
restart-db:
	docker-compose up -d dynamodb-database
