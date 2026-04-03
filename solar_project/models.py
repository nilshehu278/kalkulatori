"""Database models for solar_project."""

from flask_sqlalchemy import SQLAlchemy

# SQLAlchemy database instance
db = SQLAlchemy()


class SolarData(db.Model):
    """Stores one solar panel efficiency reading."""

    id = db.Column(db.Integer, primary_key=True, autoincrement=True)
    timestamp = db.Column(db.DateTime, nullable=False, server_default=db.func.now())
    voltage = db.Column(db.Float, nullable=False)
    current = db.Column(db.Float, nullable=False)
    power = db.Column(db.Float, nullable=False)  # Calculated as voltage * current
    efficiency = db.Column(db.Float, nullable=False)  # Efficiency percentage

    def __repr__(self) -> str:
        return (
            f"<SolarData id={self.id} voltage={self.voltage} "
            f"current={self.current} efficiency={self.efficiency}>"
        )
