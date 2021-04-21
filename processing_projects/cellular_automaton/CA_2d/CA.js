function GOL() {

  this.w = 8;
  this.columns = width/this.w;
  this.rows = height/this.w;
  this.board = new Array(this.columns);
  for (var i = 0; i < this.columns; i++) {
    this.board[i] = new Array(this.rows);
  }
  
  this.next = new Array(this.columns);
  for (var i = 0; i < this.columns; i++) {
    this.next[i] = new Array(this.rows);
  }
  
  this.init = function() {
    for (var i = 0; i < this.columns; i++) {
      for (var j = 0; j < this.rows; j++) {
        if (i === 0 || j === 0 || i == this.columns-1 || j == this.rows-1) this.board[i][j] = 0;
        else this.board[i][j] = Math.floor(random(2));
        this.next[i][j] = 0;
      }
    }
  };

  this.init();

  this.generate = function() {
    for (var x = 1; x < this.columns-1; x++) {
      for (var y = 1; y < this.rows-1; y++) {
        var neighbors = 0;

        for (var i = -1; i <= 1; i++) {
          for (var j = -1; j <= 1; j++) {
            neighbors += this.board[x+i][y+j];
          }
        }

        neighbors -= this.board[x][y];

        if ((this.board[x][y] == 1) && (neighbors <  2)) this.next[x][y] = 0; // osameli
        else if ((this.board[x][y] == 1) && (neighbors >  3)) this.next[x][y] = 0; // overpopulated
        else if ((this.board[x][y] === 0) && (neighbors == 3)) this.next[x][y] = 1; // reprodukcija
        else this.next[x][y] = this.board[x][y]; // staza
      }
    }

    var temp = this.board;
    this.board = this.next;
    this.next = temp;
  };

  this.display = function() {
    for ( var i = 0; i < this.columns;i++) {
      for ( var j = 0; j < this.rows;j++) {
        if ((this.board[i][j] == 1)) fill(255);
        else fill(0);
        stroke(0);
        rect(i*this.w, j*this.w, this.w, this.w);
      }
    }
  };
}
