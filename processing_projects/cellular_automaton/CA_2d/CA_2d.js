var gol;

function setup() {
    createCanvas(1024, 768);
    gol = new GOL();
}

function draw() {
    background(0);
    gol.generate();
    gol.display();
}

function mousePressed() {
    gol.init();
}
