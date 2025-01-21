# Stage 1: Build
FROM golang:1.20-alpine AS builder

WORKDIR /app

COPY go.mod ./
COPY go.sum ./
RUN go mod download

COPY . ./

RUN go build -o myapp main.go

# Stage 2: Minimal runtime
FROM alpine:3.18
WORKDIR /app

COPY --from=builder /app/myapp /app/myapp

EXPOSE 8080

ENTRYPOINT ["/app/myapp"]
