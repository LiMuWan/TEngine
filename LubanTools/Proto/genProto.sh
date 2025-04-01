#!/bin/sh

# Function to clear and create directory
function clear_and_create_dir() {
    if [ -d "$1" ]; then
        rm -rf "$1"
    fi
    mkdir -p "$1"
}

function main() {
    # Add the current script's directory to PATH
    PATH=$PATH:$(cd "$(dirname "$0")"; pwd)
    echo $0 ‘--’ $1
    # Define paths
    PROTO_DIR="./Proto"
    CLIENT_OUT_DIR="./out/client/"
    SERVER_OUT_DIR="./out/server/"

    # Clear and create output directories based on argument
    if [ "$1" = "client" ]; then
        clear_and_create_dir "$CLIENT_OUT_DIR"
    elif [ "$1" = "server" ]; then
        clear_and_create_dir "$SERVER_OUT_DIR"
    else
        echo "Invalid argument. Usage: $0 client|server"
		pause_on_error
        exit 1
    fi
    
    # Compile protobuf files based on argument
    if [ "$1" = "client" ]; then
        echo "compile client proto..."
        ./protoc.exe --proto_path="$PROTO_DIR" --csharp_out="$CLIENT_OUT_DIR" "$PROTO_DIR"/*.proto
    elif [ "$1" = "server" ]; then
        echo "compile server proto..."
        ./protoc --proto_path="$PROTO_DIR" --go_out="$SERVER_OUT_DIR" "$PROTO_DIR"/*.proto
    fi
    
    # Generate proto files
    echo "generating proto files..."
    python gen_proto.py "$1"
    if [ $? -ne 0 ]; then
        echo "Error generating proto files. Exiting..."
		pause_on_error
        exit 1
    fi
    
    # Additional step for client
    if [ "$1" = "client" ]; then
        echo "copy proto to client..."
        sh ./copy2Client.sh
    fi
    
    echo "all done"
	read -r
}

# Function to pause execution on error
pause_on_error() {
    echo "发生错误！按回车键继续..."
    read -r
}


main "$1"
