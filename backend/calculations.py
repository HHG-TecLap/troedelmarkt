from decimal import Decimal

def calculate_revenue(
    balance: Decimal, 
    starting_fee: Decimal,
    provision_rate: Decimal,
    /
) -> Decimal:
    return (balance-starting_fee)\
        *(1-provision_rate) if (balance-starting_fee)\
        *(1-provision_rate) > 0 else 0

def calculate_provision(
    balance: Decimal, 
    starting_fee: Decimal,
    provision_rate: Decimal,
    /
) -> Decimal:
    return (balance-starting_fee)\
        *provision_rate if (balance-starting_fee)\
        *provision_rate > 0 else 0