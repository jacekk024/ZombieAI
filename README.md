# ZombieAI

ZombieAI is a game simulator in which the player is trapped in a hall overrun by zombies. The objective of the player is to survive as long as possible, avoiding zombie attacks and trying to eliminate as many of them as possible.

The zombie simulations are created using ML-Agents, meaning that their behavior is controlled by artificial neural networks. Zombies try to avoid player's arrows while focusing on their attack. The game is dynamic and requires reflexes and strategic thinking.

## Table of Contents
* [Requirements](#requirements)
* [Assets](#assets)
* [Installation](#installation)
* [Usage](#usage)
* [License](#license)
* [Authors](#authors)
* [Contact](#contact)
 
## Requirements
* Intel Core i5 or equivalent minimum processor
* 4 GB of RAM
* Unity 2021.3.13f1 or later
* Visual Studio 2022 - 17.4.2 or later
* Nvidia Cuda 10.1 or later
* Python 3.8.10
* Pytorch 1.7.0(+cu110)
* Mlagents 0.29.0

## Assets
* Sci-fi construction kit

## Installation

Step-by-step project installation:

1. Clone the repository
2. Open the project in Unity
3. Import and install ML-Agents
4. Open the main scene

## Installation of dependencies

1. In the cloned repository create a virtual environment:

`py -m venv venv`

2. Upgrade your Preffered Installer Program - pip:

`python -m pip install --upgrade pip`

3. Download pytorch instance:

`pip install torch==1.7.0 -f https://download.pytorch.org/whl/torch_stable.html`

4. Download mlagents dependencies:

`pip install mlagents`

5. (Optional) Downgrade your protobuf package to version 3.20.00:

`pip3 install --upgrade protobuf==3.20.0 3.20.00`

## Usage

The main gameplay is based on:

1. Navigate through the hall, avoiding zombie attacks
2. Try to eliminate zombies using a bow and arrows
3. Develop a strategy to survive as long as possible

## License

This project is licensed under the MIT License. See the LICENSE file for more information.

## Authors

- Remigiusz Włoszczyński
- Jacek Kotra
- Dorian Światowy
- Kacper Kowalski

## Contact

If you have any questions or comments, please email us at remigiuszwloszczynski@gmail.com.

