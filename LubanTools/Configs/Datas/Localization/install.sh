#!/bin/bash

# Check if Python is installed
if ! command -v python3 &> /dev/null
then
    echo "Python is not installed. Please install Python first."
    read -p "Press Enter to continue..."
    exit 1
fi

# Check if pip is installed
if ! command -v pip3 &> /dev/null
then
    echo "pip is not installed. Please ensure pip is correctly installed."
    read -p "Press Enter to continue..."
    exit 1
fi

# Define packages to be installed
packages=("pandas" "openpyxl")

# Check and install packages
for package in "${packages[@]}"
do
    if ! python3 -c "import $package" &> /dev/null
    then
        echo "Installing $package..."
        if ! pip3 install "$package"
        then
            echo "Failed to install $package."
            read -p "Press Enter to continue..."
            exit 1
        fi
    else
        echo "$package is already installed."
    fi
done

echo "All necessary packages have been installed."
read -p "Press Enter to exit..."
